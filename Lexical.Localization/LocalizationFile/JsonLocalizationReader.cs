﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Lexical.Localization.Utils;
using Newtonsoft.Json.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads ".json" localization files.
    /// </summary>
    public class JsonLocalizationReader : ILocalizationFileFormat, ILocalizationKeyTreeTextReader
    {
        private readonly static JsonLocalizationReader instance = new JsonLocalizationReader();
        public static JsonLocalizationReader Instance => instance;

        protected ParameterNamePolicy namePolicy = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:\"");

        public string Extension { get; protected set; }

        public JsonLocalizationReader() : this("json")
        {
        }

        public JsonLocalizationReader(string ext)
        {
            this.Extension = ext;
        }

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            KeyTree root = new KeyTree(Key.Root);
            using (var json = new JsonTextReader(text))
                ReadJsonIntoTree(json, root, namePolicy, null);
            return root;
        }

        /// <summary>
        /// Read json token stream into <paramref name="node"/>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="node">parent node to under which add nodes</param>
        /// <param name="namePolicy"></param>
        /// <param name="correspondenceContext">(optional) place to update correspondence. If set <paramref name="json"/> must implement <see cref="JTokenReader"/>.</param>
        /// <returns></returns>
        public IKeyTree ReadJsonIntoTree(JsonReader json, IKeyTree node, IAssetKeyNamePolicy namePolicy, JsonCorrespondence correspondenceContext)
        {
            IKeyTree current = node;
            Stack<IKeyTree> stack = new Stack<IKeyTree>();
            JTokenReader tokenReader = json as JTokenReader;
            bool updateCorrespondence = correspondenceContext != null && tokenReader != null;
            while (json.Read())
            {
                switch (json.TokenType)
                {
                    case JsonToken.StartObject:
                        stack.Push(current);
                        if (updateCorrespondence) correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                        break;
                    case JsonToken.EndObject:
                        current = stack.Pop();
                        break;
                    case JsonToken.PropertyName:
                        IAssetKey key = null;
                        if (this.namePolicy.TryParse(json.Value?.ToString(), out key))
                        { 
                            current = key == null ? null : stack.Peek()?.GetOrCreate(key);
                            if (current != null && updateCorrespondence) correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                        }
                        else
                        {
                            current = null;
                        }
                        break;
                    case JsonToken.Raw:
                    case JsonToken.Date:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Float:
                    case JsonToken.Integer:
                        if (current != null)
                        {
                            string value = json.Value?.ToString();
                            if (value != null)
                            {
                                int ix = current.Values.Count;
                                current.Values.Add(value);
                                if (updateCorrespondence) correspondenceContext.Values[new KeyTreeValue(current, value, ix)] = (JValue) tokenReader.CurrentToken;
                            }
                        }
                        break;
                    case JsonToken.StartArray:
                        if (updateCorrespondence) correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                        break;
                    case JsonToken.EndArray:
                        break;
                }
            }
            return node;
        }


    }

}
﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads .xml localization files.
    /// </summary>
    public class LocalizationXmlReader : ILocalizationFileFormat, ILocalizationKeyTreeStreamReader, ILocalizationKeyTreeTextReader
    {
        /// <summary>
        /// Namespace
        /// </summary>
        public static readonly XNamespace NsDefault = "urn:lexical.fi";

        /// <summary>
        /// XName for Line element
        /// </summary>
        public static readonly XName NameLine = NsDefault + "Line";

        /// <summary>
        /// XName for document root
        /// </summary>
        public static readonly XName NameRoot = NsDefault + "Localization";

        /// <summary>
        /// URN 
        /// </summary>
        public const string URN_ = "urn:lexical.fi:";

        private readonly static LocalizationXmlReader instance = new LocalizationXmlReader();

        /// <summary>
        /// Default xml reader instance
        /// </summary>
        public static LocalizationXmlReader Instance => instance;

        /// <summary>
        /// File extension, default ".xml"
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public ILocalizationStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Xml reader settings
        /// </summary>
        protected XmlReaderSettings xmlReaderSettings;

        /// <summary>
        /// Create new xml reader
        /// </summary>
        public LocalizationXmlReader() : this("xml", LexicalStringFormat.Instance, default) { }

        /// <summary>
        /// Create new xml reader
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="valueParser"></param>
        /// <param name="xmlReaderSettings"></param>
        public LocalizationXmlReader(string extension, ILocalizationStringFormat valueParser, XmlReaderSettings xmlReaderSettings = default)
        {
            this.Extension = extension;
            this.ValueParser = valueParser as ILocalizationStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
            this.xmlReaderSettings = xmlReaderSettings ?? CreateXmlReaderSettings();
        }

        /// <summary>
        /// Read key tree from <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="namePolicy">not used</param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(XElement element, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(element, new KeyTree(Key.Root), null);

        /// <summary>
        /// Read key tree from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">not used</param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(Load(stream).Root, new KeyTree(Key.Root), null);

        /// <summary>
        /// Read key tree from <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">not used</param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(Load(text).Root, new KeyTree(Key.Root), null);

        /// <summary>
        /// Create default reader settings.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlReaderSettings CreateXmlReaderSettings()
            => new XmlReaderSettings
            {
                CheckCharacters = false,
                CloseInput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = false,
                Async = false,
                IgnoreWhitespace = false
            };

        /// <summary>
        /// Load xml document
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        protected virtual XDocument Load(Stream stream, LoadOptions loadOptions = LoadOptions.None)
        {
            //XmlReader xmlReader = new TrimmerXmlReader(stream);
            //return XDocument.Load(xmlReader, loadOptions);

            using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                return XDocument.Load(xmlReader, loadOptions);
        }

        /// <summary>
        /// Load xml document
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        protected virtual XDocument Load(TextReader textReader, LoadOptions loadOptions = LoadOptions.None)
        {
            //XmlReader xmlReader = new TrimmerXmlReader(textReader);
            //return XDocument.Load(xmlReader, loadOptions);
            using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
                return XDocument.Load(xmlReader, loadOptions);
        }

        /// <summary>
        /// Reads <paramref name="element"/>, and adds as a subnode to <paramref name="parent"/> node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <param name="correspondenceContext">(optional) Correspondence to write element-tree mappings</param>
        /// <returns>parent</returns>
        public IKeyTree ReadElement(XElement element, IKeyTree parent, XmlCorrespondence correspondenceContext)
        {
            IAssetKey key = ReadKey(element);

            if (key != null)
            {
                IKeyTree node = parent.CreateChild();
                node.Key = key;

                if (correspondenceContext != null)
                    correspondenceContext.Nodes.Put(node, element);

                foreach (XNode nn in element.Nodes())
                {
                    if (nn is XText text)
                    {
                        string trimmedXmlValue = Trim(text?.Value);
                        if (!string.IsNullOrEmpty(trimmedXmlValue))
                        {
                            IFormulationString formulationString = ValueParser.Parse(trimmedXmlValue);
                            node.Values.Add( formulationString );

                            if (correspondenceContext != null)
                                correspondenceContext.Values[new KeyTreeValue(node, formulationString, node.Values.Count - 1)] = text;
                        }
                    }
                }

                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, node, correspondenceContext);
                }
            }
            else
            {
                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, parent, correspondenceContext);
                }
            }

            return parent;
        }

        /// <summary>
        /// Trim white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string Trim(string value)
        {
            if (value == null) return null;
            int len = value.Length;
            if (len == 0) return value;
            int startIx = 0;
            for(; startIx<len; startIx++)
            {
                char ch = value[startIx];
                if (ch != 10 && ch != 13 && ch != 32 && ch != 11) break;
            }
            int endIx = len - 1;
            for(;endIx>=startIx;endIx--)
            {
                char ch = value[endIx];
                if (ch != 10 && ch != 13 && ch != 32 && ch != 11) break;
            }
            if (startIx == 0 && endIx == len - 1) return value;
            if (startIx == endIx + 1) return "";
            return value.Substring(startIx, endIx - startIx + 1);
        }

        /// <summary>
        /// Read key from <paramref name="element"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>key or null</returns>
        public IAssetKey ReadKey(XElement element)
        {
            Key key;
            // <line type="MyClass" type="something" key="something">
            if (element.Name == NameLine)
            {
                key = null;
            }
            // <type:MyClass>
            else if (element.Name.NamespaceName != null && element.Name.NamespaceName.StartsWith(URN_))
            {
                string parameterName = element.Name.NamespaceName.Substring(URN_.Length);
                string parameterValue = element.Name.LocalName;
                key = Key.Create(null, parameterName, parameterValue);
            }
            else return null;

            // Read attributes
            if (element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                    {
                        string parameterName = attribute.Name.LocalName;
                        string parameterValue = attribute.Value;

                        Match m = occuranceIndexParser.Match(parameterName);
                        Group g_name = m.Groups["name"];
                        if (m.Success && g_name.Success) parameterName = g_name.Value;
                        // Append parameter
                        key = Key.Create(key, parameterName, parameterValue);
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// Parser that extracts name from occurance index "Key_2" -> "Key", "2".
        /// </summary>
        static Regex occuranceIndexParser = new Regex("^(?<name>.*)_(?<index>\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);


        /// <summary>
        /// List all children of <paramref name="parent"/> with a readable <see cref="IAssetKey"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<IAssetKey, XElement>> ListChildrenWithKeys(XElement parent)
            => parent.Elements().Select(e => new KeyValuePair<IAssetKey, XElement>(ReadKey(e), e)).Where(line => line.Key != null);

        /*
        public class TrimmerXmlReader : XmlTextReader
        {
            public TrimmerXmlReader(Stream input) : base(input) { }
            public TrimmerXmlReader(TextReader input) : base(input) { }
            public TrimmerXmlReader(string url) : base(url) { }
            public TrimmerXmlReader(Stream input, XmlNameTable nt) : base(input, nt) { }
            public TrimmerXmlReader(TextReader input, XmlNameTable nt) : base(input, nt) { }
            public TrimmerXmlReader(string url, Stream input) : base(url, input) { }
            public TrimmerXmlReader(string url, TextReader input) : base(url, input) { }
            public TrimmerXmlReader(string url, XmlNameTable nt) : base(url, nt) { }
            public TrimmerXmlReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context) { }
            public TrimmerXmlReader(string url, Stream input, XmlNameTable nt) : base(url, input, nt) { }
            public TrimmerXmlReader(string url, TextReader input, XmlNameTable nt) : base(url, input, nt) { }
            public TrimmerXmlReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context) { }
            protected TrimmerXmlReader() { }
            protected TrimmerXmlReader(XmlNameTable nt) : base(nt) { }
            public override bool Read()
            {
                bool ok = base.Read();
                return ok;
            }
        }*/

    }

}

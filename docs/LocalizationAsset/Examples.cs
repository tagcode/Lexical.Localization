﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LocalizationAsset_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1a
                // Create localization source
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!"    },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().AddStringSource(source, "{culture:}{type:}{key}").Load();
                #endregion Snippet_1a
                IAssetKey key = new LocalizationRoot(asset).TypeSection("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.SetCulture("en"));
                Console.WriteLine(key.SetCulture("de"));
            }

            {
                #region Snippet_1b
                // Create localization source
                var source = new Dictionary<Key, string> {
                    { new Key("type", "MyController").Append("key", "hello"),                         "Hello World!" },
                    { new Key("type", "MyController").Append("key", "hello").Append("culture", "en"), "Hello World!" },
                    { new Key("type", "MyController").Append("key", "hello").Append("culture", "de"), "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().AddKeySource(source).Load();
                #endregion Snippet_1b
                IAssetKey key = new LocalizationRoot(asset).TypeSection("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.SetCulture("en"));
                Console.WriteLine(key.SetCulture("de"));
            }

            {
                #region Snippet_1c
                // Create localization source
                var source = new Dictionary<IAssetKey, string> {
                    { new LocalizationRoot().TypeSection("MyController").Key("hello"),                  "Hello World!" },
                    { new LocalizationRoot().TypeSection("MyController").Key("hello").SetCulture("en"), "Hello World!" },
                    { new LocalizationRoot().TypeSection("MyController").Key("hello").SetCulture("de"), "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().AddAssetKeySource(source).Load();
                #endregion Snippet_1c
                IAssetKey key = new LocalizationRoot(asset).TypeSection("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.SetCulture("en"));
                Console.WriteLine(key.SetCulture("de"));
            }

            {
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().AddStringSource(source, "{culture:}{type:}{key}").Load();
                #region Snippet_2
                // Extract all keys
                foreach (Key _key in asset.GetAllKeys())
                    Console.WriteLine(_key);
                #endregion Snippet_2

                #region Snippet_3
                // Keys can be filtered
                foreach (Key _key in asset.GetAllKeys(LocalizationRoot.Global.SetCulture("de")))
                    Console.WriteLine(_key);
                #endregion Snippet_3

            }
        }
    }

}

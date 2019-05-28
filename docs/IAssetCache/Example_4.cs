﻿using Lexical.Localization;
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;

namespace docs
{
    public class IAssetCache_Example_4
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new LocalizationAsset(source, LineFormat.Parameters);

            // Cache it
            asset = asset.CreateCache();

            // Issue a request which will be cached.
            ILine key = new LineRoot().Key("hello");
            IFormatString value = asset.GetString( key.Culture("en") ).GetValue();
            Console.WriteLine(value);

            // Clear cache
            asset.Reload();
            #endregion Snippet
        }
    }

}

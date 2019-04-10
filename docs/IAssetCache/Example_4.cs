﻿using Lexical.Localization;
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
            IAsset asset = new LocalizationAsset(source, ParameterNamePolicy.Instance);

            // Cache it
            asset = asset.CreateCache();

            // Issue a request which will be cached.
            IAssetKey key = new LocalizationRoot().Key("hello");
            IFormulationString str = asset.GetString( key.Culture("en") );
            Console.WriteLine(str);

            // Clear cache
            asset.Reload();
            #endregion Snippet
        }
    }

}

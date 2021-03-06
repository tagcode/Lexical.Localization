﻿using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace docs
{
    public class IAssetCache_Example_1
    {
        public static void Main(string[] args)
        {
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new StringAsset(source, LineFormat.Parameters);

            #region Snippet
            // Create cache decorator
            IAssetCache asset_cached = new AssetCache(asset).AddResourceCache().AddStringsCache().AddCulturesCache();
            #endregion Snippet

            // Assign the cached asset
            LineRoot.Global.Asset = asset_cached;
        }
    }

}

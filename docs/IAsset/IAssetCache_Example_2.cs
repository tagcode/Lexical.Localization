﻿using Lexical.Localization;
using Lexical.Localization.Asset;
using System.Collections.Generic;

namespace docs
{
    public class IAssetCache_Example_2
    {
        public static void Main(string[] args)
        {
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new StringAsset(source, LineFormat.Parameters);

            #region Snippet
            // Decorate with cache
            IAssetCache asset_cached = asset.CreateCache();
            #endregion Snippet

            // Assign the asset with cache decoration
            LineRoot.Global.Asset = asset_cached;
        }
    }

}

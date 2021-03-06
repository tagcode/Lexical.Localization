﻿using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace TutorialLibrary1
{
    public class AssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        public AssetSources() : base()
        {
            // Asset sources are added here
            Add(LocalizationSource);
        }
    }
}

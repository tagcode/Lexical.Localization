﻿using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class AssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary3-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LineFileProviderSource ExternalLocalizationSource;

        public AssetSources() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
        }

        public AssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                ExternalLocalizationSource = 
                    XmlLinesReader.Default.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(ExternalLocalizationSource);
            }
        }
    }
}

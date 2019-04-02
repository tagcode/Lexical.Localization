﻿using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class LibraryAssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LocalizationEmbeddedSource LocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LocalizationFileSource ExternalLocalizationSource = LocalizationReaderMap.Instance.FileAssetSource("Localization.xml", throwIfNotFound: false);

        public LibraryAssetSourcesB() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
            // Add optional external localization source
            Add(ExternalLocalizationSource);
        }
    }
}

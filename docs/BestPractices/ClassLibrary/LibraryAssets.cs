﻿using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public readonly IAssetSource EmbeddedLocalizationSource;

        public LibraryAssets() : base()
        {
            // Asset sources are added here
            EmbeddedLocalizationSource = XmlLocalizationReader.Instance.CreateEmbeddedAssetSource(
                    asm: GetType().Assembly,
                    resourceName: "docs.LibraryLocalization1-de.xml");

            Add(EmbeddedLocalizationSource);
        }
    }
}

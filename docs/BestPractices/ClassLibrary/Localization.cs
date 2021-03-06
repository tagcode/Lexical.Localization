﻿using Lexical.Localization;
using Lexical.Localization.Asset;

namespace TutorialLibrary1
{
    internal class Localization : LineRoot.LinkedTo
    {
        private static readonly Localization instance = new Localization(LineRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static Localization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LineRoot.Builder;

        Localization(ILineRoot linkedTo) : base(null, linkedTo, null, null, null, null, null)
        {
            // Add library's internal assets here
            Builder.AddSources(new AssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}

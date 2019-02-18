﻿using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace docs
{
    public class Ms_DependencyInjection_Example0
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet_1
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);
            #endregion Snippet_1

            #region Snippet_2
            // Create localization source
            var source = new Dictionary<string, string> { { "en:ConsoleApp1.MyController:Hello", "Hello World!" } };
            // Create asset source
            IAssetSource assetSource = new LocalizationDictionary(source).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);
            #endregion Snippet_2

            #region Snippet_3
            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Service can provide the asset
                IAsset asset = serviceProvider.GetService<IAsset>();

                // Service can provide root
                IAssetRoot root = serviceProvider.GetService<IAssetRoot>();

                // Service can provide type key
                IAssetKey typeKey = serviceProvider.GetService<IAssetKey<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").SetCulture("en").ToString();
            }
            #endregion Snippet_3
        }
    }

}

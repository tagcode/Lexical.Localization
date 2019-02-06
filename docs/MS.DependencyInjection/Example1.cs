﻿using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace docs
{
    public class Ms_DependencyInjection_Example1
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Create localization source
            var source = new Dictionary<string, string> { { "en:ConsoleApp1.MyController:Hello", "Hello World!" } };
            // Create asset source
            IAssetSource assetSource = new LocalizationStringDictionary(source).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

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
        }
    }

}

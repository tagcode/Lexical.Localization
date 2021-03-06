﻿using Lexical.Localization;
using Lexical.Localization.Asset;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class Ms_DependencyInjection_Example0
    {
        public static void Main(string[] args)
        {
            #region Snippet_1
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Configure to use CultureInfo.CurrentUICulture
            serviceCollection.AddSingleton<ICulturePolicy>(new CulturePolicy().SetToCurrentThreadUICulture().AsReadonly());            

            // Add localization services: ILineRoot, ILine<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: false,
                useGlobalInstance: false,
                addCache: false);
            #endregion Snippet_1

            #region Snippet_2
            // Create localization source
            var lines = new List<ILine> {
                LineAppender.Default.Culture("en").Type("ConsoleApp1.MyController").Key("Hello").Format("Hello World!")
            };
            // Create asset source
            IAssetSource assetSource = new StringAsset(lines).ToSource();
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
                ILineRoot root = serviceProvider.GetService<ILineRoot>();

                // Service can provide type key
                ILine typeKey = serviceProvider.GetService<ILine<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").Culture("en").ToString();
            }
            #endregion Snippet_3
        }
    }

}

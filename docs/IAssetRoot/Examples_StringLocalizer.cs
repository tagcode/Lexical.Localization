﻿using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class IAssetRoot_StringLocalizer_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet_1
            // Create localization source
            var source = new Dictionary<string, string> { { "en:MyController:hello", "Hello World!" } };
            // Create asset
            IAsset asset = new LocalizationStringDictionary(source);
            // Create culture policy
            ICulturePolicy culturePolicy = new CulturePolicy();
            // Create root
            IAssetRoot root = new StringLocalizerRoot(asset, culturePolicy);
            #endregion Snippet_1

            {
                #region Snippet_2
                // Assign as IStringLocalizer, use "MyController" as root.
                IStringLocalizer stringLocalizer = root.Section("MyController") as IStringLocalizer;
                #endregion Snippet_2
            }

            {
                #region Snippet_3
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Adapt to IStringLocalizer
                IStringLocalizer<MyController> stringLocalizer2 = 
                    stringLocalizerFactory.Create(typeof(MyController)) 
                    as IStringLocalizer<MyController>;
                #endregion Snippet_3
            }

            {
                #region Snippet_4a
                // Assign to IStringLocalizer for the class MyController
                IStringLocalizer<MyController> stringLocalizer = 
                    root.TypeSection(typeof(MyController)) 
                    as IStringLocalizer<MyController>;
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Create IStringLocalizer for the class MyController
                IStringLocalizer<MyController> stringLocalizer = 
                    stringLocalizerFactory.Create(typeof(MyController)) 
                    as IStringLocalizer<MyController>;
                #endregion Snippet_4b
            }

            {
                #region Snippet_5a            
                // Create IStringLocalizer and assign culture
                IStringLocalizer stringLocalizer = 
                    root.SetCulture("en").TypeSection<MyController>() 
                    as IStringLocalizer<MyController>;
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b            
                // Assign as IStringLocalizerFactory
                IStringLocalizerFactory stringLocalizerFactory = root as IStringLocalizerFactory;
                // Create IStringLocalizer and assign culture
                IStringLocalizer stringLocalizer = stringLocalizerFactory.Create(typeof(MyController))
                    .WithCulture(CultureInfo.GetCultureInfo("en"));
                #endregion Snippet_5b
            }

        }

        class MyController
        {

        }
    }

}

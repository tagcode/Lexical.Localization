﻿using Lexical.Localization;
using Lexical.Localization.Asset;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;
using TutorialLibrary3;

namespace TutorialProject3
{
    public class Program3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            IServiceCollection services = new ServiceCollection();

            // Install default IStringLocalizerFactory
            services.AddLexicalLocalization(
                addStringLocalizerService: true,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: false);

            // Install TutorialLibrary's IAssetSources.
            Assembly library = typeof(MyClass).Assembly;
            services.AddLibraryAssetSources(library);

            // Install additional localization that was not available in the TutorialLibrary.
            services.AddSingleton<IAssetSource>(XmlLinesReader.Default.FileAssetSource("TutorialLibrary3-fi.xml"));

            // Service MyClass
            services.AddTransient<MyClass, MyClass>();

            // Create instance container
            using (var provider = services.BuildServiceProvider())
            {
                // Create class
                MyClass myClass = provider.GetService<MyClass>();

                // Use the culture that was provided by with the class library (AssetSources)
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
                Console.WriteLine(myClass.Do());

                // Use the culture that was supplied above
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
                Console.WriteLine(myClass.Do());
            }
            #endregion Snippet
        }
    }
}

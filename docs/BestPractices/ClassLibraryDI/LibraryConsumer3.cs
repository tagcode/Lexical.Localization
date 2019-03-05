﻿using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TutorialProject
{
    public class Program3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Install localization libraries that are available in the TutorialLibrary.
            // Search for classes with [AssetSources] attribute.
            Assembly library = typeof(TutorialLibrary.MyClass2).Assembly;
            StringLocalizerRoot.Builder.AddLibraryAssetSources(library);

            // Apply sources
            StringLocalizerRoot.Builder.Build();

            // Create class
            IStringLocalizerFactory factory = StringLocalizerRoot.Global;
            IStringLocalizer<TutorialLibrary.MyClass2> localizer = StringLocalizerRoot.Global.Type<TutorialLibrary.MyClass2>();
            TutorialLibrary.MyClass2 myClass = new TutorialLibrary.MyClass2( localizer );

            // Use culture that was provided with the class library
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(myClass.Do());

            // Use culture that was supplied by this application
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(myClass.Do());
            #endregion Snippet

            Console.ReadKey();
        }
    }
}

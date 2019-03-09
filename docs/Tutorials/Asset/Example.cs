﻿using Lexical.Localization;
using System;
using System.Globalization;

namespace TutorialProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a loader
            IAsset asset = IniLocalizationReader.Instance.CreateFileAsset("HelloWorld.ini");

            // Add asset to global singleton instance
            LocalizationRoot.Builder.AddAsset(asset);
            LocalizationRoot.Builder.Build();

            // Take reference of the root
            IAssetRoot root = LocalizationRoot.Global;

            // Create key
            IAssetKey key = root.Type<Program>().Key("Hello").Inline("Hello World!");

            // Print with current culture
            Console.WriteLine(key);

            // Print with other cultures
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
            Console.WriteLine(key);

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Console.WriteLine(key);

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            Console.WriteLine(key);

            Console.ReadKey();
        }
    }
}

﻿using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class ILineFormat_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                // Create localization source
                var source = new Dictionary<string, string> { { "en/MyController/Hello", "Hello World!" } };
                // Create key name policy
                ILineFormat policy =
                    new LineParameterPrinter()
                        .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: "/") // Sorts parameters
                        .DefaultRule(true, prefixSeparator: "/"); // Default separator
                // Create asset
                IAsset asset = new StringAsset(source, policy);
                // Create key
                ILine key = new LineRoot(asset).Section("MyController").Key("Hello");
                // Retrieve string
                string str = key.Culture("en").ResolveString();
                #endregion Snippet_0a

                #region Snippet_0b
                // Test if key converted correctly to expected identity "en/Section/Key"
                string id = policy.Print(key.Culture("en"));
                #endregion Snippet_0b
            }

            {
                #region Snippet_1
                // Let's create an example key
                ILine key = new LineRoot()
                        .Location("Patches")
                        .Type("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .Culture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // "en:Patches:MyController:Errors:InvalidState"
                    string str1 = LineParameterPrinter.Default.Print(key);
                    // "en.Patches.MyController.Errors.InvalidState"
                    string str2 = LineParameterPrinter.Dot_Dot_Dot.Print(key);
                    // "Patches:MyController:Errors:InvalidState"
                    string str3 = LineParameterPrinter.None_Colon_Colon.Print(key);
                    // "en:Patches.MyController.Errors.InvalidState"
                    string str4 = LineParameterPrinter.Colon_Dot_Dot.Print(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create a custom policy 
                    ILineFormat myPolicy = new LineParameterPrinter()
                        // Enable non-canonical "Culture" parameter with "/" separator
                        .Rule("Culture", true, "", "/")
                        // Disable other non-canonical parts
                        .NonCanonicalRule(false)
                        // Enable canonical all parts with "/" separator
                        .CanonicalRule(true, "/", "")
                        // Set "Key" parameter's prefix to "/" and postfix to ".txt".
                        .Rule("Key", true, "/", ".txt");

                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.Print(key);
                    #endregion Snippet_3
                }

                {
                    #region Snippet_4
                    // Create similiar policy with LinePattern
                    ILineFormat myPolicy = new LinePattern("{culture/}{location/}{type/}{section/}[Key].txt");
                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.Print(key);
                    #endregion Snippet_4
                }

                {
                    #region Snippet_4a
                    // Create name pattern
                    ILineFormat myPolicy = new LinePattern("Patches/{Section}[-key]{-culture}.png");
                    #endregion Snippet_4a
                    // "Patches/icons-ok-de.png"
                    string str = myPolicy.Print(key);
                }
                {
                    #region Snippet_4b
                    // Create name pattern
                    ILineFormat myPolicy = new LinePattern("{location_0/}{location_1/}{location_n/}{Section}{-key}{-culture}.png");
                    // Create key
                    ILine key2 = new LineRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "Patches/20181130/icons-ok-de.png"
                    string str = myPolicy.Print(key2);
                    #endregion Snippet_4b
                }
                {
                    #region Snippet_4c
                    // Create name pattern with regular expression detail
                    ILinePattern myPolicy = new LinePattern("{location<[^/]+>/}{Section}{-key}{-culture}.png");
                    // Use its regular expression
                    Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
                    #endregion Snippet_4c
                }

            }
        }
    }

}

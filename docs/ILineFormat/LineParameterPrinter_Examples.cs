﻿using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class LineParameterPrinter_Examples
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
                ILine key = LineAppender.NonResolving
                        .Location("Patches")
                        .Section("Controllers")
                        .Type("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .Culture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // "en:Patches:Controllers:MyController:Errors:InvalidState"
                    string str1 = LineParameterPrinter.Default.Print(key);
                    // "en.Patches.Controllers.MyController.Errors.InvalidState"
                    string str2 = LineParameterPrinter.Dot_Dot_Dot.Print(key);
                    // "Patches:Controllers:MyController:Errors:InvalidState"
                    string str3 = LineParameterPrinter.None_Colon_Colon.Print(key);
                    // "en:Patches.Controllers.MyController.Errors.InvalidState"
                    string str4 = LineParameterPrinter.Colon_Dot_Dot.Print(key);
                    // "en:Patches:Controllers:MyController:Errors.InvalidState"
                    string str5 = LineParameterPrinter.Colon_Colon_Dot.Print(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create a custom policy 
                    ILineFormat myPolicy = new LineParameterPrinter()
                        // Enable non-canonical "Culture" parameter with "/" separator
                        .Rule("Culture", true, postfixSeparator: "/", order: ParameterInfos.Default.GetValue("Culture").Order)
                        // Disable other non-canonical parts
                        .NonCanonicalRule(false)
                        // Enable canonical all parts with "/" separator
                        .CanonicalRule(true, prefixSeparator: "/")
                        // Set "Key" parameter's prefix to "/"
                        .Rule("Key", true, prefixSeparator: "/", order: ParameterInfos.Default.GetValue("Key").Order);

                    // "en/Patches/MyController/Errors/InvalidState"
                    string str = myPolicy.Print(key);
                    #endregion Snippet_3
                }

            }
        }
    }

}

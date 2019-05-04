﻿using Lexical.Localization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class ParameterPattern_Examples
    {
        public static void Main(string[] args)
        {
            
            {
                #region Snippet_1
                // Let's create an example key
                ILinePart key = new LocalizationRoot()
                        .Location("Patches")
                        .Type("MyController")
                        .Section("Errors")
                        .Key("InvalidState")
                        .Culture("en");
                #endregion Snippet_1

                {
                    #region Snippet_2
                    // Create pattern
                    IParameterPolicy myPolicy = new ParameterPattern("{Culture/}{Location/}{Type/}{Section/}[Key].txt");
                    // "en/Patches/MyController/Errors/InvalidState.txt"
                    string str = myPolicy.Print(key);
                    #endregion Snippet_2
                }

                {
                    #region Snippet_3
                    // Create pattern
                    IParameterPolicy myPolicy = new ParameterPattern("Patches/{Section}[-Key]{-Culture}.png");
                    #endregion Snippet_3
                    // "Patches/Errors-InvalidState-en.png"
                    string str = myPolicy.Print(key);
                }
                {
                    #region Snippet_4a
                    // Create pattern
                    IParameterPolicy myPolicy = new ParameterPattern("{Location/}{Section}{-Key}{-Culture}.png");
                    // Create key
                    ILinePart key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "Patches/icons-ok-de.png"
                    string str = myPolicy.Print(key2);
                    #endregion Snippet_4a
                }
                {
                    #region Snippet_4b
                    // Create pattern
                    IParameterPolicy myPolicy = new ParameterPattern("{Location/}{Location/}{Location/}{Section}{-Key}{-Culture}.png");
                    // Create key
                    ILinePart key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "Patches/20181130/icons-ok-de.png"
                    string str = myPolicy.Print(key2);
                    #endregion Snippet_4b
                }
                {
                    #region Snippet_4c
                    // "[Location_n/]" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}"
                    IParameterPolicy myPolicy = new ParameterPattern("[Location_n/]{Section}{-Key}{-Culture}.png");
                    // Create key
                    ILinePart key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "Patches/20181130/icons-ok-de.png"
                    string str = myPolicy.Print(key2);
                    #endregion Snippet_4c
                }
                {
                    #region Snippet_4d
                    // Create pattern
                    IParameterPolicy myPolicy = new ParameterPattern("{Location_3}{Location_2/}{Location_1/}{Location/}{Section}{-Key}{-Culture}.png");
                    // Create key
                    ILinePart key2 = new LocalizationRoot().Location("Patches").Location("20181130").Section("icons").Key("ok").Culture("de");
                    // Converts to "20181130/Patches/icons-ok-de.png"
                    string str = myPolicy.Print(key2);
                    #endregion Snippet_4d
                }
                {
                    #region Snippet_5
                    // Create pattern with regular expression detail
                    IParameterPattern myPolicy = new ParameterPattern("{Location<[^/]+>/}{Section}{-Key}{-Culture}.png");
                    // Use its regular expression
                    Match match = myPolicy.Regex.Match("patches/icons-ok-de.png");
                    #endregion Snippet_5
                }

            }
        }
    }

}

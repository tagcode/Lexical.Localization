﻿using Lexical.Localization;
using System.Collections.Generic;

namespace docs
{
    public class LineComparer_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                IEqualityComparer<ILine> comparer = LineComparer.Default;
                #endregion Snippet_0
            }
            {
                #region Snippet_1
                ILine key = LineAppender.NonResolving.Culture("en").Key("OK");
                int hash = LineComparer.Default.GetHashCode(key);
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                ILine key1 = new LineRoot().Key("OK");
                ILine key2 = LineAppender.NonResolving.Key("OK");
                ILine key3 = LineRoot.Global.Key("OK");
                ILine key4 = StringLocalizerRoot.Global.Key("OK");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                bool equals23 = LineComparer.Default.Equals(key2, key3); // Are equal
                bool equals34 = LineComparer.Default.Equals(key3, key4); // Are equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
                int hash3 = LineComparer.Default.GetHashCode(key3);
                int hash4 = LineComparer.Default.GetHashCode(key4);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                ILine key1 = LineAppender.NonResolving.Culture("en").Key("OK");
                ILine key2 = LineAppender.NonResolving.Key("OK").Culture("en");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_3
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_4
                ILine key1 = LineAppender.NonResolving.Culture("en").Key("OK");
                ILine key2 = LineAppender.NonResolving.Culture("en").Key("OK").Culture("de");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_4
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_5
                ILine key1 = LineAppender.NonResolving.Key("OK");
                ILine key2 = LineAppender.NonResolving.Key("OK").Culture("");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
                #endregion Snippet_5
            }
            {
                #region Snippet_5b
                ILine key1 = LineAppender.NonResolving.Key("OK").Culture("fi"); // <- Selects a culture
                ILine key2 = LineAppender.NonResolving.Key("OK").Culture("").Culture("fi"); // <- Culture "" remains
                string str1 = LineFormat.Line.Print(key1);
                string str2 = LineFormat.Line.Print(key2);
                #endregion Snippet_5b
            }
            {
                #region Snippet_6
                ILine key1 = LineAppender.NonResolving.Section("").Key("OK");
                ILine key2 = LineAppender.NonResolving.Key("OK");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are not equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
                #endregion Snippet_6
            }
            {
                #region Snippet_6b
                #endregion Snippet_6b
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
            {
                #region Snippet_9
                #endregion Snippet_9
            }
        }
    }

}

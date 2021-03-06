﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Class that carries a status code result as <see cref="IString"/>.
    /// </summary>
    public class StatusString : IString
    {
        static IPlaceholder[] no_arguments = new IPlaceholder[0];
        static IStringPart[] no_parts = new IStringPart[0];
        static StatusString _null = new StatusString(null, LineStatus.StringFormatFailedNull);
        static StatusString _no_parser = new StatusString(null, LineStatus.StringFormatFailedNoParser);
        static StatusString _parse_failed = new StatusString(null, LineStatus.StringFormatFailedParse);

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusString Null => _null;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusString ParseFailed => _parse_failed;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusString NoParser => _no_parser;

        /// <summary>
        /// Get the status
        /// </summary>
        public LineStatus Status { get; internal set; }

        /// <summary>
        /// Get text
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// Get the parts
        /// </summary>
        public IStringPart[] Parts => no_parts;

        /// <summary>
        /// Get arguments
        /// </summary>
        public IPlaceholder[] Placeholders => no_arguments;

        /// <summary>
        /// Get format provider.
        /// </summary>
        public IFormatProvider FormatProvider => null;

        /// <summary>
        /// 
        /// </summary>
        public IStringFormat StringFormat => null;

        /// <summary>
        /// Crate string for status
        /// </summary>
        /// <param name="text"></param>
        /// <param name="status"></param>
        public StatusString(string text, LineStatus status)
        {
            this.Status = status;
            this.Text = text;
        }

    }
}

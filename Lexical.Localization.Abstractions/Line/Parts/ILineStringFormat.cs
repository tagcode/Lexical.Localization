﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with format provider.
    /// </summary>
    public interface ILineStringFormat : ILine
    {
        /// <summary>
        /// (Optional) The assigned format provider.
        /// </summary>
        IStringFormat StringFormat { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append string format.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="stringFormat"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineStringFormat StringFormat(this ILine line, IStringFormat stringFormat)
            => line.Append<ILineStringFormat, IStringFormat>(stringFormat);

        /// <summary>
        /// Append string format.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="stringFormat"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineHint StringFormat(this ILine line, string stringFormat)
            => line.Append<ILineHint, string, string>("StringFormat", stringFormat);

        /// <summary>
        /// Append string format.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="stringFormat"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineStringFormat StringFormat(this ILineFactory lineFactory, IStringFormat stringFormat)
            => lineFactory.Create<ILineStringFormat, IStringFormat>(null, stringFormat);

        /// <summary>
        /// Append string format.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="stringFormat"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If could not be appended</exception>
        public static ILineHint StringFormat(this ILineFactory lineFactory, string stringFormat)
            => lineFactory.Create<ILineHint, string, string>(null, "StringFormat", stringFormat);

        /// <summary>
        /// Search linked list and finds the effective (left-most) <see cref="ILineStringFormat"/> key.
        /// 
        /// Returns parameter "StringFormat" value as <see cref="IStringFormat"/>, if <paramref name="resolver"/> is provided.
        /// 
        /// If implements <see cref="ILineStringFormat"/> returns the type. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver">(optional) type resolver that resolves "IStringFormat" parameter into type. Returns null, if could not resolve, exception if resolve fails</param>
        /// <returns>type info or null</returns>
        /// <exception cref="Exception">from <paramref name="resolver"/></exception>
        public static IStringFormat FindStringFormat(this ILine line, IResolver resolver = null)
        {
            IStringFormat type = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineStringFormat part && part.StringFormat != null) { type = part.StringFormat; continue; }
                if (resolver != null && l is ILineParameterEnumerable lineParameters)
                {
                    IStringFormat tt = null;
                    foreach (ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "StringFormat" && parameter.ParameterValue != null)
                        {
                            tt = resolver.Resolve<IStringFormat>(parameter.ParameterValue);
                            if (tt != null) break;
                        }
                    }
                    if (tt != null) { type = tt; continue; }
                }
                if (resolver != null && l is ILineParameter lineParameter && lineParameter.ParameterName == "StringFormat" && lineParameter.ParameterValue != null)
                {
                    IStringFormat t = resolver.Resolve<IStringFormat>(lineParameter.ParameterValue);
                    if (t != null) type = t;
                }
            }
            return type;
        }

        /// <summary>
        /// Get effective (closest to root) type name.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type name or null</returns>
        public static string FindStringFormatName(this ILine line)
        {
            string result = null;
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                        if (lineParameter.ParameterName == "StringFormat" && lineParameter.ParameterValue != null) { result = lineParameter.ParameterValue; break; }
                }
                else if (part is ILineParameter parameter && parameter.ParameterName == "StringFormat" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                else if (part is ILineStringFormat key && key.StringFormat != null) result = key.StringFormat.Name;
            }
            return result;
        }


    }
}

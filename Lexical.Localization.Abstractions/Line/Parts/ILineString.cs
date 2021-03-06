﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization string value.
    /// </summary>
    public interface ILineString : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        IString String { get; set; }
    }
    
    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineString"/> part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString String(this ILine part, IString value)
            => part.Append<ILineString, IString>(value);

        /// <summary>
        /// Create <see cref="ILineString"/> part.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString String(this ILineFactory lineFactory, IString value)
            => lineFactory.Create<ILineString, IString>(null, value);

        /// <summary>
        /// Append "String" hint. 
        /// 
        /// StringResolver will parse the hint using the active StringFormat.
        /// If there is no active string format, then CSharp format is used
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineHint String(this ILine part, String value)
            => part.Append<ILineHint, string, string>("String", value);

        /// <summary>
        /// Create "String" hint.
        /// 
        /// StringResolver will parse the hint using the active StringFormat.
        /// If there is no active string format, then CSharp format is used
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineHint String(this ILineFactory lineFactory, String value)
            => lineFactory.Create<ILineHint, string, string>(null, "String", value);

        /// <summary>
        /// Get the <see cref="IString"/> of a <see cref="ILineString"/>.
        /// 
        /// If parameter "String" exists and <paramref name="resolver"/> is provided then value is resolved using
        /// the default string format or the format that can be found.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver">(optional) type resolver that resolves "IStringFormat" parameter into type. Returns null, if could not resolve, exception if resolve fails</param>
        /// <param name="fallbackStringFormat">(optional) fallback string format to use in case line didnt have one</param>
        /// <returns>value</returns>
        /// <exception cref="LineException">error parsing</exception>
        public static IString GetString(this ILine line, IResolver resolver = null, IStringFormat fallbackStringFormat = null)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineString valuePart && valuePart.String != null) return valuePart.String;
                if (resolver != null && part is ILineParameterEnumerable lineParameters)
                {
                    foreach(ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "String" && parameter.ParameterValue != null)
                        {
                            IStringFormat stringFormat = line.FindStringFormat(resolver) ?? fallbackStringFormat;
                            return stringFormat.Parse(parameter.ParameterValue);
                        }
                    }
                }
                if (part is ILineParameter lineParameter && lineParameter.ParameterName == "String" && lineParameter.ParameterValue != null)
                {
                    IStringFormat stringFormat = line.FindStringFormat(resolver) ?? fallbackStringFormat;
                    return stringFormat.Parse(lineParameter.ParameterValue);
                }
            }
            return StatusString.Null;
        }

        /// <summary>
        /// Try get string that implements <see cref="IString"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <param name="resolver">(optional) type resolver that resolves "StringFormat" parameter into type. Returns null, if could not resolve, exception if resolve fails</param>
        /// <returns>true if part was found</returns>
        public static bool TryGetString(this ILine line, out IString result, IResolver resolver = null)
        {
            try
            {
                for (ILine part = line; part != null; part = part.GetPreviousPart())
                {
                    if (part is ILineString valuePart && valuePart.String != null) { result = valuePart.String; return true; }
                    if (resolver != null && part is ILineParameterEnumerable lineParameters)
                    {
                        foreach (ILineParameter parameter in lineParameters)
                        {
                            if (parameter.ParameterName == "String" && parameter.ParameterValue != null)
                            {
                                IStringFormat stringFormat = line.FindStringFormat(resolver);
                                result = stringFormat.Parse(parameter.ParameterValue);
                                return true;
                            }
                        }
                    }
                    if (resolver != null && part is ILineParameter lineParameter && lineParameter.ParameterName == "String" && lineParameter.ParameterValue != null)
                    {
                        IStringFormat stringFormat = line.FindStringFormat(resolver);
                        result = stringFormat.Parse(lineParameter.ParameterValue);
                        return true;
                    }
                }
                result = new StatusString(null, LineStatus.StringFormatFailedNull);
                return false;
            } catch(Exception)
            {
                result = new StatusString(null, LineStatus.FailedUnknownReason);
                return false;
            }
        }

        /// <summary>
        /// Finds a part that implements <see cref="ILineString"/> or is a parameter "Value.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetStringPart(this ILine line, out ILine result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineString valuePart && valuePart.String != null) { result = part; return true; }
                if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "String" && parameter.ParameterValue != null) { result = part; return true; }
                    }
                }
                if (part is ILineParameter lineParameter && lineParameter.ParameterName == "String" && lineParameter.ParameterValue != null) { result = part; return true; }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Finds a part that implements <see cref="ILineString"/> or is a parameter "String".
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetStringText(this ILine line, out string result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineString valuePart && valuePart.String != null) { result = valuePart.String.Text; return true; }
                if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "String" && parameter.ParameterValue != null) { result = parameter.ParameterValue; return true; }
                    }
                }
                if (part is ILineParameter lineParameter && lineParameter.ParameterName == "String" && lineParameter.ParameterValue != null) { result = lineParameter.ParameterValue; return true; }
            }
            result = default;
            return false;
        }

    }

}


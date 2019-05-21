﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// The default localization formatter implementation.
    /// </summary>
    public class StringResolver : IStringResolver
    {
        private static StringResolver instance = new StringResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static StringResolver Instance => instance;

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IFormatString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        public IFormatString ResolveFormatString(ILine key)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineString"/> with format arguments applied.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public LineString ResolveString(ILine line)
        {
            // Extract parameters from line
            LineFeatures features = new LineFeatures();
            features.ScanFeatures(line);



            throw new NotImplementedException();
        }

        /*
                /// <summary>
                /// Resolve the format string. 
                /// 
                /// Uses the following algorithm:
                ///   1. Either explicitly assigned culture or <see cref="ICulturePolicy"/> from <see cref="ILineExtensions.FindCulturePolicy(ILine)"/>.
                ///   2. Try key as is.
                ///   
                ///      a. Search inlines with culture
                ///      b. Search asset with culture
                /// </summary>
                /// <param name="key"></param>
                /// <returns>format string (without formulating it) or null</returns>
                public LineString ResolveString(ILine key)
                {
                    // If there is no explicitly assigned culture in the key, try cultures from culture policy
                    string explicitCulture = key.GetCultureName();
                    IEnumerable<CultureInfo> cultures = null;
                    bool rootCultureTried = false;
                    if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        foreach (CultureInfo culture in cultures)
                        {
                            bool rootCulture = culture.Name == "";
                            rootCultureTried |= rootCulture;
                            // 
                            ILine culture_key = rootCulture ? key : key.Culture(culture);
                            // Try inlines
                            if (languageString == null && inlines != null) inlines.TryGetValue(culture_key, out languageString);
                            // Try key
                            if (languageString == null) languageString = culture_key.TryGetString();
                            // Return
                            if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                        }
                    }

                    if (!rootCultureTried)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        // Try inlines with key
                        if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                        // Try asset with key
                        if (languageString == null) languageString = key.TryGetString();
                        // Return
                        if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                    }

                    return new LineString(key, null, LineStatus.NoResult);
                }

                /// <summary>
                /// Resolve language string. 
                /// 
                /// Uses the following algorithm:
                ///   1. Either explicitly assigned culture or <see cref="ICulturePolicy"/> from <see cref="ILineExtensions.FindCulturePolicy(ILine)"/>.
                ///   2. Try key as is.
                ///   
                ///      a. Search inlines with plurality and culture
                ///      b. Search inlines with culture
                ///      c. Search asset with plurality and culture
                ///      d. Search asset with culture
                ///   
                ///   3. Then try to formulate the string with assigned arguments, e.g. "Error (Code=0xFEEDF00D)"
                /// </summary>
                /// <param name="key"></param>
                /// <returns>If key has <see cref="ILineFormatArgsPart"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
                /// If key didn't have <see cref="ILineFormatArgsPart"/> part, then return the format string "Error (Code=0x{0:X8})".
                /// otherwise return null</returns>
                public LineString ResolveString(ILine key)
                {
                    // Get args
                    object[] format_args = key.FindFormatArgs();

                    // Plurality key when there is only one numeric argument. e.g. "Key:N:One"
                    ILine pluralityKey = null;
                    // Pluarlity key for all argument permutations (whether argument is provided or not)
                    IEnumerable<ILine> pluralityKeys = null;
                    if (format_args != null)
                    {
                        for (int argumentIndex = 0; argumentIndex < format_args.Length; argumentIndex++)
                        {
                            object o = format_args[argumentIndex];
                            if (o == null) continue;
                            string pluralityKind = GetPluralityKind(o);
                            if (pluralityKind == null) continue;
                            if (pluralityKey == null)
                            {
                                pluralityKey = key.N(argumentIndex, pluralityKind);
                            }
                            else
                            {
                                pluralityKey = null;
                                pluralityKeys = CreatePluralityKeyPermutations(key: key, maxArgumentCount: Plurality_.MAX_NUMERIC_ARGUMENTS_TO_PERMUTATE, args: format_args); // 2^5 = 32 keys
                                break;
                            }
                        }
                    }

                    // If there is no explicitly assigned culture in the key, try cultures from culture policy
                    bool rootCultureTried = false;
                    string explicitCulture = key.GetCultureName();
                    IEnumerable<CultureInfo> cultures = null;
                    if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        foreach (CultureInfo culture in cultures)
                        {
                            bool rootCulture = culture.Name == "";
                            rootCultureTried |= rootCulture;
                            // Append culture
                            ILine pluralityKey_with_culture = rootCulture ? pluralityKey : pluralityKey?.Culture(culture);
                            // Try inlines with plurality key
                            if (languageString == null && inlines != null && pluralityKey_with_culture != null) inlines.TryGetValue(pluralityKey_with_culture, out languageString);
                            // Try inlines with plurality key permutations
                            if (languageString == null && inlines != null && pluralityKeys != null)
                            {
                                foreach (ILine _pluralityKey in pluralityKeys)
                                    if (inlines.TryGetValue(_pluralityKey.Culture(culture), out languageString) && languageString != null) break;
                            }
                            // Append culture
                            ILine key_with_culture = rootCulture ? key : key.Culture(culture);
                            // Try inlines with fallback key
                            if (languageString == null && inlines != null) inlines.TryGetValue(key_with_culture, out languageString);
                            // Try asset with plurality key
                            if (languageString == null && pluralityKey_with_culture != null) languageString = pluralityKey_with_culture.TryGetString();
                            // Try asset with plurality key permutations
                            if (languageString == null && pluralityKeys != null)
                            {
                                foreach (ILine _pluralityKey in pluralityKeys)
                                {
                                    languageString = _pluralityKey.TryGetString();
                                    if (languageString != null) break;
                                }
                            }
                            // Try asset with fallback key
                            if (languageString == null) languageString = key_with_culture.TryGetString();
                            // Formulate language string
                            if (languageString != null && format_args != null) return Format(key, culture, languageString, format_args);
                            // Return format without arguments applied
                            if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                        }
                    }

                    // Try key as is
                    if (!rootCultureTried)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        // Try inlines with plurality key
                        if (languageString == null && inlines != null && pluralityKey != null) inlines.TryGetValue(pluralityKey, out languageString);
                        // Try inlines with plurality key permutations
                        if (languageString == null && inlines != null && pluralityKeys != null)
                        {
                            foreach (ILine _pluralityKey in pluralityKeys)
                                if (inlines.TryGetValue(_pluralityKey, out languageString) && languageString != null) break;
                        }
                        // Try inlines with fallback key
                        if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                        // Try asset with plurality key
                        if (languageString == null && pluralityKey != null) languageString = pluralityKey.TryGetString();
                        // Try asset with plurality key permutations
                        if (languageString == null && pluralityKeys != null)
                        {
                            foreach (ILine _pluralityKey in pluralityKeys)
                            {
                                languageString = _pluralityKey.TryGetString();
                                if (languageString != null) break;
                            }
                        }
                        // Try asset with fallback key
                        if (languageString == null) languageString = key.TryGetString();
                        // Formulate language string
                        if (languageString != null && format_args != null) return Format(key, rootCulture, languageString, format_args);
                        // Return format without applying arguments
                        if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                    }

                    return new LineString(key, null, LineStatus.NoResult);
                }

                static CultureInfo rootCulture = CultureInfo.GetCultureInfo("");

                /// <summary>
                /// Apply <paramref name="args"/> into <paramref name="formatString"/>.
                /// </summary>
                /// <param name="key"></param>
                /// <param name="culture"></param>
                /// <param name="formatString"></param>
                /// <param name="args"></param>
                /// <returns></returns>
                static LineString Format(ILine key, CultureInfo culture, IFormatString formatString, object[] args)
                {
                    // Convert to strings
                    string[] arg_strings = new string[formatString.Placeholders.Length];
                    for(int i=0; i< formatString.Placeholders.Length; i++)
                    {
                        IPlaceholder argumentFormat = formatString.Placeholders[i];
                        int argIndex = argumentFormat.ArgumentIndex;
                        if (args!=null && argIndex >= 0 && argIndex < args.Length)
                        {
                            arg_strings[i] = Format(args[argIndex], argumentFormat.Format, culture);
                        }
                        else
                        {
                            arg_strings[i] = "";
                        }
                    }

                    // Count characters
                    int c = 0;
                    foreach(var part in formatString.Parts)
                    {
                        if (part.Kind == FormatStringPartKind.Text) c += part.Length;
                        else if (part.Kind == FormatStringPartKind.Placeholder && part is IPlaceholder arg) c += arg_strings[arg.ArgumentsIndex].Length;
                    }

                    // Put together string
                    char[] chars = new char[c];
                    int ix = 0;
                    foreach (var part in formatString.Parts)
                    {
                        if (part.Kind == FormatStringPartKind.Text)
                        {
                            formatString.Text.CopyTo(part.Index, chars, ix, part.Length);
                            ix += part.Length;
                        }
                        else if (part.Kind == FormatStringPartKind.Placeholder && part is IPlaceholder arg)
                        {
                            string str = arg_strings[arg.ArgumentsIndex];
                            if (str != null)
                            {
                                str.CopyTo(0, chars, ix, str.Length);
                                ix += str.Length;
                            }
                        }
                    }


                    return new LineString(key, new String(chars), 0);
                }

                static string Format(object o, string format, IFormatProvider culture)
                {
                    if (o == null) return "";
                    if (o is IFormattable formattable) return formattable.ToString(format, culture);
                    if (culture.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter_)
                        return customFormatter_.Format(format, o, culture);
                    return culture == null ? String.Format("{0:" + format + "}", o) : String.Format(culture, "{0:" + format + "}", o);
                }

                /// <summary>
                /// Gets plurality for number in <paramref name="o"/>. 
                /// 
                /// See: <see cref="Plurality"/>.
                /// </summary>
                /// <param name="o"></param>
                /// <returns>null, "Zero", "One", "Plural"</returns>
                protected static string GetPluralityKind(object o)
                {
                    // null
                    if (o == null) return null;

                    Type type = o.GetType();

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Byte:
                            byte _byte = (byte)o;
                            if (_byte == 0) return Plurality_.Zero;
                            if (_byte == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.SByte:
                            sbyte _sbyte = (sbyte)o;
                            if (_sbyte == 0) return Plurality_.Zero;
                            if (_sbyte == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Decimal:
                            decimal _decimal = (decimal)o;
                            if (_decimal == 0) return Plurality_.Zero;
                            if (_decimal == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Int16:
                            Int16 _int16 = (Int16)o;
                            if (_int16 == 0) return Plurality_.Zero;
                            if (_int16 == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Int32:
                            Int32 _int32 = (Int32)o;
                            if (_int32 == 0) return Plurality_.Zero;
                            if (_int32 == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Int64:
                            Int64 _int64 = (Int64)o;
                            if (_int64 == 0L) return Plurality_.Zero;
                            if (_int64 == 1L) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Single:
                            Single _single = (Single)o;
                            if (_single == 0.0f) return Plurality_.Zero;
                            if (_single == 1.0f) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Double:
                            double _double = (double)o;
                            if (_double == 0.0) return Plurality_.Zero;
                            if (_double == 1.0) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.UInt16:
                            UInt16 _uint16 = (UInt16)o;
                            if (_uint16 == 0) return Plurality_.Zero;
                            if (_uint16 == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.UInt32:
                            UInt32 _uint32 = (UInt32)o;
                            if (_uint32 == 0) return Plurality_.Zero;
                            if (_uint32 == 1) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.UInt64:
                            UInt64 _uint64 = (UInt32)o;
                            if (_uint64 == 0UL) return Plurality_.Zero;
                            if (_uint64 == 1UL) return Plurality_.One;
                            return Plurality_.Plural;
                        case TypeCode.Object:
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return GetPluralityKind(Nullable.GetUnderlyingType(type));
                            return null;
                    }
                    return null;
                }

                private static string GetPluralityKind<T>()
                    => GetPluralityKind(typeof(T));

                /// <summary>
                /// Create plurality key permutations. 
                /// 
                /// If argument count exceeds <paramref name="maxArgumentCount"/> then returns single arguments only:
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   :N2:(Zero/One/Plural)
                ///   :N3:(Zero/One/Plural)
                /// 
                /// Result for two arguments:
                /// 
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   
                /// Result for three arguments:
                /// 
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   :N2:(Zero/One/Plural)
                /// 
                /// Number of elements: n = numbericArguments ^ 2 - 1
                /// </summary>
                /// <param name="key"></param>
                /// <param name="maxArgumentCount"></param>
                /// <param name="args">arguments, only numberic arguments are used</param>
                /// <returns>all permutation keys or null</returns>
                private static IEnumerable<ILine> CreatePluralityKeyPermutations(ILine key, int maxArgumentCount, object[] args)
                {
                    if (maxArgumentCount <= 0) return null;
                    // Gather info: (argumentIndex, pluralityKind)
                    (int, string)[] argInfos = new (int, string)[32];
                    int argumentCount = 0;
                    for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
                    {
                        object o = args[argumentIndex];
                        if (o == null) continue;
                        string pluralityKind = GetPluralityKind(o);
                        if (pluralityKind == null) continue;
                        argInfos[argumentCount] = (argumentIndex, pluralityKind);
                        argumentCount++;
                        if (argumentCount >= 32) return null;
                    }
                    if (argumentCount == 0) return null;

                    // Return single numeric argument keys only
                    if (argumentCount > maxArgumentCount) return _CreatePluralityKeysSingleArgumentOnly(key, argInfos, argumentCount);

                    // Return all permutations for numeric arguments
                    return _CreatePluralityKeyPermutations(key, argInfos, argumentCount);
                }

                private static IEnumerable<ILine> _CreatePluralityKeyPermutations(ILine key, (int, string)[] argInfos, int argumentCount)
                {
                    int allBits = (int)(1 << argumentCount) - 1;
                    for (int bits = allBits; bits > 0; bits--)
                    {
                        ILine pluralityKey = key;
                        for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                        {
                            if ((bits & (1 << argumentIndex)) == 0) continue;
                            pluralityKey = pluralityKey.N(argumentIndex, argInfos[argumentIndex].Item2);
                        }
                        yield return pluralityKey;
                    }
                }

                private static IEnumerable<ILine> _CreatePluralityKeysSingleArgumentOnly(ILine key, (int, string)[] argInfos, int argumentCount)
                {
                    for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                    {
                        yield return key.N(argumentIndex, argInfos[argumentIndex].Item2);
                    }
                }
                */
    }    

}

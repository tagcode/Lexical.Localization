﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationReaderExtensions_
    {
        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAsset StreamAsset(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyTree(stream, namePolicy).ToKeyLines().AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyLines(stream, namePolicy).AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationStringAsset(fileFormat.ReadStringLines(stream, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy), namePolicy);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="streamSource"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAssetSource StreamAssetSource(this ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
            => new StreamProviderAssetSource(fileFormat, streamSource, namePolicy, prefix, suffix);

    }

}
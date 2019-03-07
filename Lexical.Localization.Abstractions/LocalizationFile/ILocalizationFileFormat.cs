﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Represents a localization file format.
    /// </summary>
    public interface ILocalizationFileFormat
    {
        /// <summary>
        /// Extension of the file format without separator. e.g. "xml".
        /// </summary>
        string Extension { get; }
    }

    /// <summary>
    /// Signals that file format can be read localization files.
    /// </summary>
    public interface ILocalizationReader : ILocalizationFileFormat { }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationKeyLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. </param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization into tree format format a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationKeyTreeStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structuer.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationKeyLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationKeyTreeTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationStringLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, string>> ReadStringLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read string key-values from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationStringLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read string key-values</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<string, string>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    [Flags]
    public enum WriteFlags : UInt32
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow to adds new entries.
        /// </summary>
        Add = 1,
        /// <summary>
        /// Allow to removes entires that no longer exist.
        /// </summary>
        Remove = 2,
        /// <summary>
        /// Allow to modify values of existing entries.
        /// </summary>
        Modify = 4,
        /// <summary>
        /// Overwrite contents of previous content.
        /// </summary>
        Overwrite = 8,

        /// <summary>
        /// Update-add, but doesn't remove
        /// </summary>
        Update = Add|Modify,

        /// <summary>
        /// Synchronize one file to another
        /// </summary>
        Synchronize = Add | Modify | Remove,
    }

    /// <summary>
    /// Signals that file format can write localization files.
    /// </summary>
    public interface ILocalizationWriter : ILocalizationFileFormat { }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILocalizationStringLinesTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteStringLines(IEnumerable<KeyValuePair<string, string>> lines, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILocalizationStringLinesStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteStringLines(IEnumerable<KeyValuePair<string, string>> lines, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILocalizationKeyLinesTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteKeyLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILocalizationKeyLinesStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteKeyLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization tree structure to streams.
    /// </summary>
    public interface ILocalizationKeyTreeStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="tree"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteKeyTree(IKeyTree tree, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    /// <summary>
    /// Writer that can write localization tree structure to text writer.
    /// </summary>
    public interface ILocalizationKeyTreeTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="srcText">(optional) source text, used if previous content is updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        void WriteKeyTree(IKeyTree tree, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags);
    }

    public static class LocalizationFileFormatExtensions
    {

    }

}
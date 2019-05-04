﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lexical.Localization.Internal;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// This class is a context-free implementation of <see cref="ILinePart"/>. 
    /// It can be used as a reference, but not as a provider of localization content.
    /// It is used as a key for persisting and comparison.
    /// 
    /// This class has one parameter name and a value, and it can carry a link to previous node.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public partial class Key : ILinePart, ILineParameter, ILineParameterAssignable, IEnumerable<KeyValuePair<string, string>>, IEquatable<Key>, ILineDefaultHashCode
    {
        private static readonly Key root = new Key("", "");

        ILinePartAppender ILinePart.Appender => Appender.Instance;
        /// <summary></summary>
        public class Appender : ILinePartAppender2<ILineParameter, string, string>,
            ILinePartAppender2<ILineCanonicallyComparedKey, string, string>,
            ILinePartAppender2<ILineNonCanonicallyComparedKey, string, string>
        {
            static readonly Appender instance = new Appender();
            /// <summary></summary>
            public static Appender Instance => instance;
            ILineParameter ILinePartAppender2<ILineParameter, string, string>.Append(ILinePart previous, string parameterName, string parameterKey)
                => ((Key)previous).AppendParameter(parameterName, parameterKey);
            ILineCanonicallyComparedKey ILinePartAppender2<ILineCanonicallyComparedKey, string, string>.Append(ILinePart prevKey, string parameterName, string parameterValue)
                => new Key.Canonical((Key)prevKey, parameterName, parameterValue);
            ILineNonCanonicallyComparedKey ILinePartAppender2<ILineNonCanonicallyComparedKey, string, string>.Append(ILinePart prevKey, string parameterName, string parameterValue)
                => new Key.NonCanonical((Key)prevKey, parameterName, parameterValue);
        }

        /// <summary>
        /// Default root key.
        /// </summary>
        public static Key Root => root;

        /// <summary>
        /// Parameter name, e.g. "Culture"
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Parameter value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Link to previous key in a linked list.
        /// </summary>
        public Key Previous;

        ILinePart ILinePart.PreviousPart => Previous;
        string ILineParameter.ParameterValue => Value;
        public string ParameterName => Name;

        /// <summary>
        /// Cached parameters array.
        /// </summary>
        string[] parameters;

        static string[] empty = new string[0];

        /// <summary>
        /// Parameters array. Contains one or no elements.
        /// </summary>
        public string[] Parameters => parameters ?? (parameters = String.IsNullOrEmpty(Name) ? empty : new string[] { Name });

        /// <summary>
        /// Create proxy root implementation of <see cref="ILinePart"/>. Contains one parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public Key(string parameterName, string parameterValue)
        {
            this.Name = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            this.Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
        }

        /// <summary>
        /// Create proxy implementation of <see cref="ILinePart"/>. Contains one parameter.
        /// </summary>
        /// <param name="previous">(optional) previous link</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public Key(Key previous, string parameterName, string parameterValue)
        {
            this.Name = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            this.Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
            this.Previous = previous;
        }

        /// <summary>
        /// Create a new link in a new reference.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name=""></param>
        /// <returns>new reference with a new key</returns>
        public Key Append(string parameterName, string parameterValue)
            => Create(this, parameterName, parameterValue);

        public ILineParameter AppendParameter(string parameterName, string parameterValue)
            => Create(this, parameterName, parameterValue);

        public static Key Create(string parameterName, string parameterValue)
            => Create(null, parameterName, parameterValue);

        public static Key Create(Key prevKey, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = default)
        {
            IParameterInfo info;
            if ((parameterInfos ?? ParameterInfos.Default).TryGetValue(parameterName, out info))
            {
                if (info.IsCanonical) return new Key.Canonical(prevKey, parameterName, parameterValue);
                if (info.IsNonCanonical) return new Key.NonCanonical(prevKey, parameterName, parameterValue);
                return new Key(prevKey, parameterName, parameterValue);
            }
            else
            {
                // Guess .. or throw error?
                return new Key.Canonical(prevKey, parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Convert parameters to key.
        /// </summary>
        /// <param name="parameters">(optional) parameters</param>
        /// <returns>key or null if contained no parameters</returns>
        public static Key CreateFromParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null) return null;
            Key result = null;
            // Convert "match" parameters to key
            foreach (var parameter in parameters)
                result = Key.Create(result, parameter.Key, parameter.Value);
            return result;
        }

        /// <summary>
        /// Create by copying from a source key.
        /// </summary>
        /// <param name="key">(optional) key to copy</param>
        /// <returns>key, new key, or null is <paramref name="key"/> contained no parameters</returns>
        public static Key CreateFrom(ILinePart key)
        {
            if (key == null) return null;
            if (key is Key k) return k;
            Key result = null;
            key.VisitParameters(_copyVisitor, ref result);
            return result;
        }
        static KeyParameterVisitor<Key> _copyVisitor = copyVisitor;
        static void copyVisitor(string parameterName, string parameterValue, ref Key result)
            => result = Key.Create(result, parameterName, parameterValue);

        /// <summary>
        /// Concatenate two keys.
        /// </summary>
        /// <param name="anotherKey"></param>
        /// <returns></returns>
        public Key Concat(Key anotherKey)
        {
            Key result = this;
            if (anotherKey != null)
            {
                foreach (Key k in anotherKey.ToArray())
                    result = result.Append(k.Name, k.Value);
            }
            return result;
        }

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="ILineNonCanonicallyComparedKey"/>.
        /// </summary>
        [DebuggerDisplay("{ToString()}")]
        public class NonCanonical : Key, ILineNonCanonicallyComparedKey
        {
            public NonCanonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public NonCanonical(Key previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        /// <summary>
        /// Proxy implementation of non-canonical parameter. Implements <see cref="ILineCanonicallyComparedKey"/>.
        /// </summary>
        [DebuggerDisplay("{ToString()}")]
        public class Canonical : Key, ILineCanonicallyComparedKey
        {
            public Canonical(string parameterName, string parameterValue) : base(parameterName, parameterValue) { }
            public Canonical(Key previous, string parameterName, string parameterValue) : base(previous, parameterName, parameterValue) { }
        }

        bool defaultHashcodeCalculated;
        int defaultHashCode;
        int ILineDefaultHashCode.GetDefaultHashCode()
        {
            if (defaultHashcodeCalculated) return defaultHashCode;
            defaultHashCode = LineComparer.Default.CalculateHashCode(this);
            Thread.MemoryBarrier();
            defaultHashcodeCalculated = true;
            return defaultHashCode;
        }

        bool IEquatable<Key>.Equals(Key other)
            => LineComparer.Default.Equals(this, other);

        public override int GetHashCode()
        {
            if (defaultHashcodeCalculated) return defaultHashCode;
            defaultHashCode = LineComparer.Default.CalculateHashCode(this);
            Thread.MemoryBarrier();
            defaultHashcodeCalculated = true;
            return defaultHashCode;
        }

        public override bool Equals(object obj)
            => obj is Key other ? LineComparer.Default.Equals(this, other) : false;

        /// <summary>
        /// Prints the key in "parameterName:parameterValue:..." format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => ParameterPolicy.Instance.Print(this);

        /// <summary>
        /// Comparer that compares a single key instance, not the whole chain.
        /// </summary>
        public class Comparer : IEqualityComparer<Key>, IComparer<Key>
        {
            private static Comparer instance = new Comparer();
            private static IEqualityComparer<Key[]> arrayComparer = new ArrayComparer<Key>(new Comparer());

            public static Comparer Default => instance;
            public static IEqualityComparer<Key[]> Array => arrayComparer;

            public readonly IComparer<string> parameterNameComparer;
            public readonly IComparer<string> parameterValueComparer;

            public Comparer(IComparer<string> parameterNameComparer = default, IComparer<string> parameterValueComparer = default)
            {
                this.parameterNameComparer = parameterNameComparer ?? StringComparer.InvariantCultureIgnoreCase;
                this.parameterValueComparer = parameterValueComparer ?? StringComparer.InvariantCultureIgnoreCase;
            }

            public int Compare(Key x, Key y)
            {
                string x_comparand = x.Name, y_comparand = y.Name;
                int o = parameterNameComparer.Compare(x_comparand, y_comparand);
                if (o != 0) return -o;
                o = parameterValueComparer.Compare(x.Value, y.Value);
                return o;
            }

            public bool Equals(Key x, Key y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name && x.Value == y.Value;
            }

            public int GetHashCode(Key obj)
            {
                int hash = 24342;
                if (obj.Value != null) { hash ^= obj.Value.GetHashCode(); hash *= 137; }
                if (obj.Name != null) { hash ^= obj.Name.GetHashCode(); hash *= 137; }
                return hash;
            }
        }

        /// <summary>
        /// Create an array of parameters from head towards tail.
        /// </summary>
        /// <param name="includeNonCanonical">include all keys that implement ILocalizationKeyNonCanonicallyCompared</param>
        /// <returns>array of keys</returns>
        public Key[] ToArray(bool includeNonCanonical = true)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    count++;
            }

            // Create result
            Key[] result = new Key[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    result[--ix] = k;
            }

            return result;
        }

        /// <summary>
        /// Enumerate from head towards tail.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, string>>)ToKeyValueArray()).GetEnumerator();

        /// <summary>
        /// Enumerate from head towards tail.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => ToKeyValueArray().GetEnumerator();

        string[] cachedKeyArray;

        public string[] ToKeyArray()
            => cachedKeyArray ?? (cachedKeyArray = _toKeyArray(true));

        public string[] ToKeyArray(bool includeNonCanonical)
            => includeNonCanonical ? ToKeyArray() : _toKeyArray(includeNonCanonical);

        string[] _toKeyArray(bool includeNonCanonical)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    count++;
            }

            if (count == 0) return empty;

            // Create result
            string[] result = new string[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    result[--ix] = k.Name;
            }

            return result;
        }

        KeyValuePair<string, string>[] cachedKeyValueArray;

        public KeyValuePair<string, string>[] ToKeyValueArray()
            => cachedKeyValueArray ?? (cachedKeyValueArray = _toKeyValueArray(true));

        public KeyValuePair<string, string>[] ToKeyValueArray(bool includeNonCanonical)
            => includeNonCanonical ? ToKeyValueArray() : _toKeyValueArray(includeNonCanonical);

        KeyValuePair<string, string>[] _toKeyValueArray(bool includeNonCanonical)
        {
            // Count the number of keys
            int count = 0;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    count++;
            }

            // Create result
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
            int ix = count;
            for (Key k = this; k != null; k = k.Previous)
            {
                if ((includeNonCanonical && k is ILineNonCanonicallyComparedKey) || k is ILineCanonicallyComparedKey)
                    result[--ix] = new KeyValuePair<string, string>(k.Name, k.Value);
            }

            return result;
        }

    }

}

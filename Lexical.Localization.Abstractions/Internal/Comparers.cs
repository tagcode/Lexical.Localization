﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Order comparer between KeyValuePair&lt;Key, Value&gt;.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class KeyValuePairComparer<Key, Value> : IComparer<KeyValuePair<Key, Value>>
    {
        private static KeyValuePairComparer<Key, Value> instance;

        /// <summary>
        /// Default comparer instance.
        /// </summary>
        public static KeyValuePairComparer<Key, Value> Default => instance ?? (instance = new KeyValuePairComparer<Key, Value>(Comparer<Key>.Default, Comparer<Value>.Default));

        /// <summary>
        /// Comparer for key part.
        /// </summary>
        public readonly IComparer<Key> keyComparer;

        /// <summary>
        /// Comparer for value part.
        /// </summary>
        public readonly IComparer<Value> valueComparer;

        /// <summary>
        /// Create cinoarer
        /// </summary>
        /// <param name="keyComparer"></param>
        /// <param name="valueComparer"></param>
        public KeyValuePairComparer(IComparer<Key> keyComparer, IComparer<Value> valueComparer)
        {
            this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
            this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        }

        /// <summary>
        /// Compare two pairs.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
        {
            int compare = 0;
            compare = keyComparer.Compare(x.Key, y.Key);
            if (compare != 0) return compare;
            compare = valueComparer.Compare(x.Value, y.Value);
            if (compare != 0) return compare;
            return 0;
        }
    }

    /// <summary>
    /// Equality comparer between <see cref="KeyValuePair{Key, Value}"/>.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class KeyValuePairEqualityComparer<Key, Value> : IEqualityComparer<KeyValuePair<Key, Value>>
    {
        private static KeyValuePairEqualityComparer<Key, Value> instance;

        /// <summary>
        /// Default comparer instance.
        /// </summary>
        public static KeyValuePairEqualityComparer<Key, Value> Default => instance ?? (instance = new KeyValuePairEqualityComparer<Key, Value>(EqualityComparer<Key>.Default, EqualityComparer<Value>.Default));

        /// <summary>
        /// Comparer for key part.
        /// </summary>
        public readonly IEqualityComparer<Key> keyComparer;

        /// <summary>
        /// Comparer for value part.
        /// </summary>
        public readonly IEqualityComparer<Value> valueComparer;

        /// <summary>
        /// Create comparer.
        /// </summary>
        /// <param name="keyComparer"></param>
        /// <param name="valueComparer"></param>
        public KeyValuePairEqualityComparer(IEqualityComparer<Key> keyComparer, IEqualityComparer<Value> valueComparer)
        {
            this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
            this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        }

        /// <summary>
        /// Compare two parts for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
        {
            if (!keyComparer.Equals(x.Key, y.Key)) return false;
            if (!valueComparer.Equals(x.Value, y.Value)) return false;
            return true;
        }

        /// <summary>
        /// Calculate hashcode for pair.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(KeyValuePair<Key, Value> obj)
            => (obj.Key == null ? 0 : 11 * obj.Key.GetHashCode()) + (obj.Value == null ? 0 : 13 * obj.Value.GetHashCode());
    }

    /// <summary>
    /// Comparer that forwards calls to Equals and HashCode methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualsComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Test equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            bool xnull = object.ReferenceEquals(x, null), ynull = object.ReferenceEquals(y, null);
            if (xnull && ynull) return true;
            if (xnull || ynull) return false;
            return x.Equals(y);
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }

    /// <summary>
    /// Comparer that makes object reference comparison.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferenceComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Compare object references.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y) => object.ReferenceEquals(x, y);

        /// <summary>
        /// Forward to object GetHashCode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj) => obj == null ? 0 : obj.GetHashCode();
    }

    /// <summary>
    /// Array elements comparer.
    /// </summary>
    /// <typeparam name="Element"></typeparam>
    public class ArrayComparer<Element> : IEqualityComparer<Element[]>
    {
        /// <summary>
        /// Element comparer.
        /// </summary>
        public readonly IEqualityComparer<Element> elementComparer;

        /// <summary>
        /// Create comparer.
        /// </summary>
        /// <param name="elementComparer"></param>
        public ArrayComparer(IEqualityComparer<Element> elementComparer)
        {
            this.elementComparer = elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
        }

        /// <summary>
        /// Hash initial value.
        /// </summary>
        public const int FNVHashBasis = unchecked((int)2166136261);

        /// <summary>
        /// Hash factor.
        /// </summary>
        public const int FNVHashPrime = 16777619;

        /// <summary>
        /// Compare arrays.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Element[] x, Element[] y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            int len = x.Length;
            for (int i = 0; i < len; i++)
                if (!elementComparer.Equals(x[i], y[i])) return false;
            return true;
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public int GetHashCode(Element[] array)
        {
            if (array == null) return 0;
            int result = FNVHashBasis;
            foreach (Element e in array)
            {
                if (e != null) result ^= elementComparer.GetHashCode(e);
                result *= FNVHashPrime;
            }
            return result;
        }
    }
}

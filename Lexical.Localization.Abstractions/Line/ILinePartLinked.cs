﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// A visitor delegate that is used when part is visited.
    /// Visitation starts from tail and proceeds towards root.
    /// Visitation is stack allocated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="part"></param>
    /// <param name="data"></param>
    public delegate void LinePartVisitor<T>(ILine part, ref T data);

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Enumerate from tail to root.
        /// </summary>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> EnumerateToRoot(this ILine tail)
        {
            for (ILine l = tail; l != null; l = l.GetPreviousPart())
                yield return l;
        }

        /// <summary>
        /// Visit line parts from root towards tail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tail"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        public static void VisitFromRoot<T>(this ILine tail, LinePartVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILine prevPart = tail.GetPreviousPart();
            if (prevPart != null) VisitFromRoot(prevPart, visitor, ref data);
            // Pop from stack in reverse order
            visitor(tail, ref data);
        }

        /// <summary>
        /// Array of <see cref="ILine"/> parts from root towards tail.
        /// </summary>
        /// <param name="tail"></param>
        /// <param name="whereFilter">(optional) where filter</param>
        /// <returns>array of parts</returns>
        public static ILine[] ToArray(this ILine tail, Func<ILine, bool> whereFilter = null)
        {
            // Count the number of parts
            int count = 0;
            if (whereFilter != null)
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    if (whereFilter(p)) count++;
            }
            else
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart()) count++;
            }

            // Create result
            ILine[] result = new ILine[count];
            int ix = count - 1;
            if (whereFilter != null)
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    result[ix--] = p;
            else
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    if (whereFilter(p))
                        result[ix--] = p;
            }

            return result;
        }

        /// <summary>
        /// Finds part that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tail"></param>
        /// <returns>T or null</returns>
        public static T Find<T>(this ILine tail) where T : ILine
        {
            for (; tail != null; tail = tail.GetPreviousPart())
                if (tail is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Finds part that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tail"></param>
        /// <returns>T</returns>
        /// <exception cref="LineException">if T is not found</exception>
        public static T Get<T>(this ILine tail) where T : ILine
        {
            for (; tail != null; tail = tail.GetPreviousPart())
                if (tail is T casted) return casted;
            throw new LineException(tail, $"{typeof(T).FullName} is not found.");
        }

        /// <summary>
        /// Finds part that implements T when walking towards root, start from previous part.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <returns>T or null</returns>
        public static T FindPrev<T>(this ILine part) where T : ILine
        {
            for (ILine k = part.GetPreviousPart(); k != null; k = k.GetPreviousPart())
                if (k is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Scan part towards root, returns <paramref name="index"/>th part from tail (0=tail, count-1=root)
        /// </summary>
        /// <param name="tail"></param>
        /// <param name="index">the index of part to return starting from tail.</param>
        /// <returns>part</returns>
        /// <exception cref="IndexOutOfRangeException">if <paramref name="index"/> goes over root</exception>
        public static ILine GetAt(this ILine tail, int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            for (int i = 0; i < index; i++)
                tail = tail.GetPreviousPart();
            if (tail == null) throw new IndexOutOfRangeException();
            return tail;
        }

    }
}

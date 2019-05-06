﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization string value.
    /// </summary>
    public interface ILineValue : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        IFormulationString Value { get; }
    }

    /// <summary>
    /// Localization string value of a <see cref="ILine"/>.
    /// </summary>
    public interface ILineValuePart : ILine, ILineValue
    {
    }

    /// <summary></summary>
    public static partial class ILineValueExtensions
    {
        /// <summary>
        /// Append <see cref="ILineValue"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILine Value(this ILine part, IFormulationString value)
            => part.Append<ILineValuePart, IFormulationString>(value);

        /// <summary>
        /// Get the <see cref="IFormulationString"/> of a <see cref="ILineValue"/>, or
        /// the last <see cref="IFormulationString"/> of <see cref="ILineValuePart"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>value or null</returns>
        public static IFormulationString GetValue(this ILine line)
        {
            if (line is ILineValue value) return value.Value;
            if (line is ILine part)
            {
                for (ILine p = part; p != null; p = p.GetPreviousPart())
                    if (part is ILineValue valuePart && valuePart.Value != null) return valuePart.Value;
            }
            return null;
        }

    }


}

    
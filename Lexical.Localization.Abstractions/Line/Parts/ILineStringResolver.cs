﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineStringResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        IStringResolver StringResolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver Resolver(this ILine line, IStringResolver resolver)
            => line.Append<ILineStringResolver, IStringResolver>(resolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver Resolver(this ILineFactory lineFactory, IStringResolver resolver)
            => lineFactory.Create<ILineStringResolver, IStringResolver>(null, resolver);
    }
}

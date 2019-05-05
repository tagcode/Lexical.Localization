﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// "Culture" key that carries <see cref="CultureInfo"/>. 
    /// </summary>
    [Serializable]
    public class LineCultureKey : LineKey, ILineKeyCulture
    {
        /// <summary>
        /// CultureInfo, null if non-standard culture.
        /// </summary>
        protected CultureInfo culture;

        /// <summary>
        /// Culture property
        /// </summary>
        public CultureInfo Culture { get => culture; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public LineCultureKey(ILinePartAppender appender, ILinePart prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineCultureKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culture = info.GetValue("Culture", typeof(CultureInfo)) as CultureInfo;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Culture", culture);
        }
    }

    public partial class LinePartAppender : ILinePartAppender1<ILineKeyCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ILineKeyCulture Append(ILinePart previous, CultureInfo culture)
            => new LineCultureKey(this, previous, culture);
    }
    /*
    /// <summary>
    /// "Culture" key that carries <see cref="CultureInfo"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerCultureKey : StringLocalizerKey, ILineKeyCulture
    {
        /// <summary>
        /// CultureInfo, null if non-standard culture.
        /// </summary>
        protected CultureInfo culture;

        /// <summary>
        /// Culture property
        /// </summary>
        public CultureInfo Culture { get => culture; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public StringLocalizerCultureKey(ILinePartAppender appender, ILinePart prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerCultureKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culture = info.GetValue("Culture", typeof(CultureInfo)) as CultureInfo;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Culture", culture);
        }
    }

    public partial class StringLocalizerPartAppender : ILinePartAppender1<ILineKeyCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ILineKeyCulture Append(ILinePart previous, CultureInfo culture)
            => new StringLocalizerCultureKey(this, previous, culture);
    }
*/
}

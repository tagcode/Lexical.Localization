﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineStringResolver"/>. 
    /// </summary>
    [Serializable]
    public class LineStringResolver : LineBase, ILineStringResolver, ILineArgument<IStringResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IStringResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IStringResolver StringResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IStringResolver Argument0 => resolver;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public LineStringResolver(ILineFactory appender, ILine prevKey, IStringResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineStringResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("StringResolver", typeof(IStringResolver)) as IStringResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StringResolver", resolver);
        }
    }

    public partial class LineAppender : ILineFactory<ILineStringResolver, IStringResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IStringResolver LineResolver, out ILineStringResolver line)
        {
            line = new LineStringResolver(appender, previous, LineResolver);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries <see cref="ILineStringResolver"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerLineResolver : StringLocalizerBase, ILineStringResolver, ILineArgument<IStringResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IStringResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IStringResolver StringResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IStringResolver Argument0 => resolver;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public StringLocalizerLineResolver(ILineFactory appender, ILine prevKey, IStringResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerLineResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("StringResolver", typeof(IStringResolver)) as IStringResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StringResolver", resolver);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineStringResolver, IStringResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IStringResolver LineResolver, out ILineStringResolver StringLocalizer)
        {
            StringLocalizer = new StringLocalizerLineResolver(appender, previous, LineResolver);
            return true;
        }
    }


}

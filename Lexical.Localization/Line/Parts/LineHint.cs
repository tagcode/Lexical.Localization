﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// </summary>
    [Serializable]
    public class LineHint : LineParameterBase, ILineArgument<ILineHint, string, string>, ILineHint
    {
        string ILineArgument<ILineHint, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineHint, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineHint(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineHint(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class LineAppender : ILineFactory<ILineHint, string, string>
    {
        /// <summary>
        /// Append <see cref="LineHint"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineHint result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineHint castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            // Return as parameter
            result = new LineHint(appender, previous, parameterName, parameterValue);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that represents a parameter key-value pair.
    /// </summary>
    [Serializable]
    public class StringLocalizerHint : StringLocalizerParameterBase, ILineArgument<ILineHint, string, string>, ILineHint
    {
        string ILineArgument<ILineHint, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineHint, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public StringLocalizerHint(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerHint(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineHint, string, string>
    {
        /// <summary>
        /// Append <see cref="StringLocalizerHint"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineHint result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineHint castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            result = new StringLocalizerHint(appender, previous, parameterName, parameterValue);
            return true;
        }
    }

}

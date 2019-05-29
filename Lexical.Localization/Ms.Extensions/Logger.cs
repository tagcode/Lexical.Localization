﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using Microsoft.Extensions.Logging;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILine"/>.
    /// </summary>
    public static partial class ILineLoggerMsExtensions
    {
        /// <summary>
        /// Append <paramref name="logger"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger ILogger(this ILine line, ILogger logger)
            => line.Logger(new LineILogger(logger));
    }


    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="ILogger"/>.
    /// </summary>
    public class LineILogger : IObserver<LineString>
    {
        ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public LineILogger(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            logger = null;
        }

        /// <summary>
        /// Formatter produced exception
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Write status
            if (_logger.IsEnabled(LogLevel.Error)) _logger.LogError(error, error.Message, error.Data);
        }

        /// <summary>
        /// Formatter supplies format result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            int severity = value.Severity;
            // Write status
            if (_logger.IsEnabled(LogLevel.Trace) && severity == 0) { _logger.LogError(value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == 1) { _logger.LogWarning(value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= 2) { _logger.LogError(value.DebugInfo); return; }
        }
    }
}

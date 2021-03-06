﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Result of an operation that resolves a <see cref="ILine"/> into a string within an executing context, such as one that includes current active culture.
    /// </summary>
    public struct LineString
    {
        /// <summary>
        /// Return string <see cref="String"/>.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator string(LineString str)
            => str.Value;

        /// <summary>
        /// Convert string to <see cref="LineString"/>.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator LineString(string str)
            => str == null ?
            new LineString(null, LineStatus.StringFormatFailedNull) :
            new LineString(null, str, LineStatus.StringFormatOkString);

        /// <summary>
        /// Return status.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator LineStatus(LineString str)
            => str.Status;

        /// <summary>
        /// Convert from status code.
        /// </summary>
        /// <param name="status"></param>
        public static implicit operator LineString(LineStatus status)
            => new LineString(null, status);

        /// <summary>
        /// Status code
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// (optional) The line that was requested to be resolved.
        /// </summary>
        public ILine Line;

        /// <summary>
        /// Resolved string.
        /// 
        /// Depending on what was requested, either format string as is, or formatted string with arguments applied to the format.
        /// 
        /// Null, if value was not available.
        /// </summary>
        public String Value;

        /// <summary>
        /// Unexpected exception.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Highest severity value out of each category.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity Severity => Status.Severity();

        /// <summary>
        /// Tests if there is a result, be that successful or an error.
        /// </summary>
        public bool HasResult => Status != LineStatus.NoResult;

        /// <summary>
        /// Result has ok state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Produced ok value.
        /// </summary>
        public bool Ok => Status.Ok();

        /// <summary>
        /// Result has warning state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Warning state has a value, but there was something occured during the resolve that may need attention.
        /// </summary>
        public bool Warning => Status.Warning();

        /// <summary>
        /// Result has error state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Error state has some kind of fallback value, but it is bad quality.
        /// </summary>
        public bool Error => Status.Error();

        /// <summary>
        /// Result has failed state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Failed state has no value.
        /// </summary>
        public bool Failed => Status.Failed();

        /// <summary>
        /// Create new localization string.
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="value">resolved string</param>
        /// <param name="status">resolve reslut</param>
        public LineString(ILine line, string value, LineStatus status)
        {
            Line = line;
            Exception = null;
            Value = value;
            Status = status;
        }

        /// <summary>
        /// Create new localization string.
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="error">error</param>
        /// <param name="status">resolve reslut</param>
        public LineString(ILine line, Exception error, LineStatus status)
        {
            Line = line;
            Value = null;
            Exception = error;
            Status = status;
        }

        /// <summary>
        /// Create new localization string.
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="status">resolve reslut</param>
        public LineString(ILine line, LineStatus status)
        {
            Line = line;
            Exception = null;
            Value = null;
            Status = status;
        }

        /// <summary>
        /// Return Value or ""
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Value ?? "";

        /// <summary>
        /// Print debug information about the formatting result.
        /// </summary>
        /// <returns></returns>
        public string DebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                // Append status
                Status.AppendFlags(sb);

                // Append key
                if (Line != null)
                {
                    sb.Append(" ");
                    StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                    Line.GetParameterParts<StructList12<ILineParameter>>(ref list);
                    for(int i = list.Count - 1; i >= 0; i--)
                    {
                        var parameter = list[i];
                        if (parameter.ParameterName == "String") continue;
                        if (i < list.Count - 1) sb.Append(':');
                        sb.Append(parameter.ParameterName);
                        sb.Append(':');
                        sb.Append(parameter.ParameterValue);
                    }
                }

                // Append result
                if (Value != null)
                {
                    sb.Append(" = \"");
                    sb.Append(Value);
                    sb.Append("\"");
                }

                // Compile string
                return sb.ToString();
            }
        }
    }

}

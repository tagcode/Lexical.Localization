﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Resolver;
using System;
using System.Reflection;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolves string format class name to string format.
    /// </summary>
    public class StringFormatResolver : BaseResolver<IStringFormat>, IParameterResolver
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly Lazy<StringFormatResolver> instance = new Lazy<StringFormatResolver>();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static StringFormatResolver Default => instance.Value;

        /// <summary>
        /// Parameter names supported by this resolver.
        /// </summary>
        static string[] parameterNames = new string[] { "StringFormat", "String" };

        /// <summary>
        /// Parameter Name
        /// </summary>
        public string[] ParameterNames => parameterNames;

        /// <summary>
        /// Create type resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public StringFormatResolver() : this(DefaultAssemblyResolver, DefaultTypeResolver)
        {
        }

        /// <summary>
        /// Create type resolver.
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        public StringFormatResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver) : base(assemblyLoader, typeResolver)
        {
        }

        /// <summary>
        /// Dispose or clear resolver.
        /// </summary>
        public override void Dispose()
        {
            if (this == Default)
            {
                // Don't dispose the global static instance, but clear its cache.
                cache.Clear();
            } else
            {
                // Continue disposing
                base.Dispose();
            }
        }

        /// <summary>
        /// Resolve "StringFormat" and "String" into respective line arguments.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArgument"></param>
        /// <returns></returns>
        public bool TryResolveParameter(ILine previous, string parameterName, string parameterValue, out ILineArgument resolvedLineArgument)
        {
            if (parameterValue != null && parameterValue != "")
            {
                if (parameterName == "String")
                {
                    IStringFormat stringFormat = previous.FindStringFormat() ?? CSharpFormat.Default;
                    IString value = stringFormat.Parse(parameterValue);
                    resolvedLineArgument = new LineArgument<ILineString, IString>(value);
                    return true;
                }
                else if (parameterName == "StringFormat")
                {
                    IStringFormat stringFormat;
                    if (TryResolve(parameterValue, out stringFormat))
                    {
                        resolvedLineArgument = new LineArgument<ILineStringFormat, IStringFormat>(stringFormat);
                        return true;
                    }
                }
            }

            resolvedLineArgument = default;
            return false;
        }
    }
}

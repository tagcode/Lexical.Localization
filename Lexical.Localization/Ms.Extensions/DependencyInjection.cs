﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization
{
    /// <summary></summary>
    public static partial class MsDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the following Lexical.Localization services:
        ///    <see cref="ILineRoot"/>
        ///    <see cref="ILine{T}"/>
        ///    <see cref="IAssetBuilder"/>
        ///    
        /// If <paramref name="addCulturePolicyService"/> is true a <see cref="ICulturePolicy"/> is added,
        /// otherwise <see cref="ICulturePolicy"/> must be added to the service collection.
        /// 
        /// Further services are needed:
        ///    <see cref="IAssetSource"/> one or more.
        ///    
        /// If <paramref name="addStringLocalizerService"/> is true, the following services are added:
        ///    <see cref="IStringLocalizerFactory"/>
        ///    <see cref="IStringLocalizer{T}"/>
        ///    
        /// If <paramref name="useGlobalInstance"/> is true, then uses global <see cref="LineRoot"/>.
        /// 
        /// 
        /// After this call, the <paramref name="serviceCollection"/> still needs to be populated with 
        /// instances of <see cref="IAssetSource"/>, such as:
        ///     <see cref="ResourceStringDictionary"/>
        ///     <see cref="StringAsset"/>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="addStringLocalizerService"></param>
        /// <param name="addCulturePolicyService">Add instance of <see cref="CulturePolicy"/></param>
        /// <param name="useGlobalInstance"></param>
        /// <param name="addCache"></param>
        /// <param name="addLogger">Add adapter for <see cref="ILogger"/></param>
        /// <returns></returns>
        public static IServiceCollection AddLexicalLocalization(
            this IServiceCollection serviceCollection,
            bool addStringLocalizerService = true,
            bool addCulturePolicyService = true,
            bool useGlobalInstance = false,
            bool addCache = false,
            bool addLogger = true
            )
        {
            // ILineRoot
            if (useGlobalInstance)
            {
                // Use StringLocalizerRoot as ILineRoot
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<ILineRoot>(
                    s =>
                    {
                        IAsset asset = s.GetService<IAsset>(); // DO NOT REMOVE
                        ICulturePolicy culturePolicy = s.GetService<ICulturePolicy>();
                        if (culturePolicy != null) StringLocalizerRoot.Global.CulturePolicy = culturePolicy;
                        return StringLocalizerRoot.Global;
                    }
                    ));
            }
            else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<ILineRoot, StringLocalizerRoot>());
            }

            // ILineFactory
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<ILineFactory>(StringLocalizerAppender.NonResolving));

            // ILineAsset
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<ILineAsset>(s => s.GetService<ILineRoot>() as ILineAsset));

            // ICulturePolicy
            if (addCulturePolicyService)
            {
                if (useGlobalInstance)
                {
                    serviceCollection.TryAdd(ServiceDescriptor.Singleton<ICulturePolicy>(StringLocalizerRoot.Global.CulturePolicy));
                }
                else
                {
                    serviceCollection.TryAdd(ServiceDescriptor.Singleton<ICulturePolicy>(
                        s => new CulturePolicy().SetToCurrentThreadCulture()
                        ));
                }
            }

            // ILineResolver
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<IStringResolver>(StringResolver.Default));

            // ILogger<ILine>
            if (addLogger) serviceCollection.AddLoggerAdapter();

            // IAssetBuilder
            if (useGlobalInstance)
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder>(s =>
                {
                    IEnumerable<IAssetSource> assetSources = s.GetServices<IAssetSource>();
                    IAssetBuilder builder = StringLocalizerRoot.Builder;
                    builder.AddSources(assetSources);
                    return builder;
                }));
            }
            else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder>(s =>
                {
                    // Get IAssetSource services
                    IEnumerable<IAssetSource> assetSources = s.GetServices<IAssetSource>();
                    // Get IEnumerable<ILibraryAssetSources> services
                    IEnumerable<IAssetSources> libraryAssetSourcesLists = s.GetServices<IAssetSources>();
                    // Concatenate
                    if (libraryAssetSourcesLists != null)
                    {
                        foreach (IEnumerable<IAssetSource> assetSources_ in libraryAssetSourcesLists)
                            assetSources = assetSources == null ? assetSources_ : assetSources.Concat(assetSources_);
                    }
                    // Take distinct
                    if (assetSources != null) assetSources = assetSources.Distinct();
                    // Is it still empty
                    if (assetSources == null) assetSources = new IAssetSource[0];
                    // Create builder
                    AssetBuilder.OneBuildInstance builder = new AssetBuilder.OneBuildInstance(assetSources);
                    return builder;
                }));
            }

            // IAsset
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAsset>(s => s.GetService<IAssetBuilder>().Build()));

            // ILine<>
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(ILine<>), typeof(StringLocalizerType<>)));

            // IStringLocalizer<>
            // IStringLocalizerFactory
            if (addStringLocalizerService)
            {
                // Service request for IStringLocalizer<> instances
                serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IStringLocalizer<>), typeof(StringLocalizerType<>)));
                // Service reqeust for IStringLocalizerFactory
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IStringLocalizerFactory>(s =>
                {
                    ILineRoot localizationRoot = s.GetService<ILineRoot>();
                    // Use the StringLocalizerKey or StringLocalizerRoot implementation from th service.
                    if (localizationRoot is IStringLocalizerFactory casted) return casted;
                    // Create new root that implements IStringLocalizerFactory and acquires asset and policy with delegate
                    return new StringLocalizerRoot.LinkedTo(StringLocalizerAppender.NonResolving, localizationRoot);
                }));
            }

            // Add cache
            if (addCache)
            {
                // Add cache
                serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddCulturesCache()));
            }

            return serviceCollection;
        }

        /// <summary>
        /// Adds logger service to root.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        static IServiceCollection AddLoggerAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IObserver<LineString>), s =>
            {
                ILogger<ILine> logger = s.GetService<ILogger<ILine>>();
                IObserver<LineString> adapter = logger == null ? null : new MSLogger(logger);
                return adapter;
            }));
            return serviceCollection;
        }

        /// <summary>
        /// Search for classes that implement <see cref="ILibraryAssetSources"/> in <paramref name="library"/>.
        /// Instantiates them and adds as services of <see cref="IAssetSources"/>, which will be picked up
        /// by services installed by <see cref="AddLexicalLocalization"/>.
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="library">(optional) library to scan</param>
        public static IServiceCollection AddLibraryAssetSources(this IServiceCollection services, Assembly library)
        {
            if (library == null) return services;

            IEnumerable<ServiceDescriptor> librarysAssetSourceServices =
                    library
                    .GetExportedTypes()
                    .Where(t => typeof(ILibraryAssetSources).IsAssignableFrom(t))
                    .Select(t => new ServiceDescriptor(typeof(IAssetSources), t, ServiceLifetime.Singleton));

            foreach (ServiceDescriptor serviceDescriptor in librarysAssetSourceServices)
                services.Add(serviceDescriptor);

            return services;
        }
    }
}

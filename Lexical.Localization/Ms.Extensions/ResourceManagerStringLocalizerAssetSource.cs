﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A source that creates <see cref="ResourceManagerStringLocalizer"/> and adapts it into <see cref="IAsset"/>.
    /// </summary>
    public class ResourceManagerStringLocalizerAssetSource : IAssetSource
    {
        /// <summary>
        /// ResourceManager to use Type
        /// </summary>
        public readonly Type type;

        /// <summary>
        /// ResourceManager to use location and basename
        /// </summary>
        public readonly String location, basename;

        /// <summary>
        /// Resource manager to search from resource path
        /// </summary>
        public readonly string resourcePath;

        /// <summary>
        /// 
        /// </summary>
        public readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Create a source that uses .resx files that are inteded for a specific type.
        /// </summary>
        /// <param name="resourcePath">(optional) hint, for embedded resouce path</param>
        /// <param name="type"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IAssetSource Type(string resourcePath, Type type, ILoggerFactory loggerFactory)
            => new ResourceManagerStringLocalizerAssetSource(resourcePath, null, null, type, loggerFactory);

        /// <summary>
        /// Create a source that uses .resx files in a specific assembly and filename.
        /// </summary>
        /// <param name="location">assembly name</param>
        /// <param name="resourcePath">(optional) hint, for embedded resource path</param>
        /// <param name="basename">.resx filename</param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IAssetSource Location(string location, string resourcePath, string basename, ILoggerFactory loggerFactory)
            => new ResourceManagerStringLocalizerAssetSource(resourcePath, location, basename, null, loggerFactory);

        ResourceManagerStringLocalizerAssetSource(string resourcePath, string location, string basename, Type type, ILoggerFactory loggerFactory)
        {
            this.type = type;
            this.location = location;
            this.basename = basename;
            this.resourcePath = resourcePath;
        }

        /// <summary>
        /// Add resource manager to asset.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
        {
            if (type != null) list.Add(ResourceManagerStringLocalizerAsset.Create(resourcePath, type, loggerFactory));
            else list.Add(ResourceManagerStringLocalizerAsset.Create(location, resourcePath, basename, loggerFactory));
        }

        /// <summary>
        /// No action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;
    }

    /// <summary>
    /// A source that creates <see cref="ResourceManagerStringLocalizerFactory"/> and adapts it into <see cref="IAsset"/>.
    /// </summary>
    public class ResourceManagerStringLocalizerFactoryAssetSource : IAssetSource
    {
        /// <summary>
        /// Path to resource
        /// </summary>
        public readonly string resourcePath;

        /// <summary>
        /// Logger factory
        /// </summary>
        public readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="loggerFactory"></param>
        public ResourceManagerStringLocalizerFactoryAssetSource(string resourcePath, ILoggerFactory loggerFactory)
        {
            this.resourcePath = resourcePath;
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
        {
            list.Add( ResourceManagerStringLocalizerAsset.CreateFactory(resourcePath, loggerFactory) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;
    }

}

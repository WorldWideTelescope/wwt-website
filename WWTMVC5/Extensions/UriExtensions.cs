//-----------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Stream.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Gets the application path of the given URI.
        /// </summary>
        /// <param name="thisObject">Current Uri object.</param>
        /// <returns>Application path.</returns>
        public static string GetApplicationPath(this Uri thisObject)
        {
            string resourcesPath = string.Empty;

            if (thisObject != null)
            {
                //// Remove "ResourceService" from URL.
                resourcesPath = thisObject.AbsoluteUri.Remove(thisObject.AbsoluteUri.LastIndexOf(thisObject.Segments[thisObject.Segments.Length - 1], StringComparison.OrdinalIgnoreCase));
                //// Remove "/" from URL.
                resourcesPath = resourcesPath.Remove(thisObject.AbsoluteUri.LastIndexOf(thisObject.Segments[thisObject.Segments.Length - 2], StringComparison.OrdinalIgnoreCase));
            }

            return resourcesPath;
        }

        /// <summary>
        /// Gets the Server link of the given URI.
        /// </summary>
        /// <param name="thisObject">Current Uri object.</param>
        /// <returns>Server Link path.</returns>
        public static string GetServerLink(this Uri thisObject)
        {
            string resourcesPath = string.Empty;

            if (thisObject != null)
            {
                resourcesPath = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", thisObject.Scheme, thisObject.Host);
                if (!resourcesPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    resourcesPath += "/";
                }
            }

            return resourcesPath;
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="UnityDependencyResolver.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Microsoft.Research.EarthOnline.NotificationService
{
    /// <summary>
    /// Class representing the unity dependency resolver used for dependency injection.
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Instance of unity container.
        /// </summary>
        private readonly IUnityContainer unityContainer;

        /// <summary>
        /// Initializes a new instance of the UnityDependencyResolver class.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        public UnityDependencyResolver(IUnityContainer container)
        {
            this.unityContainer = container;
        }

        /// <summary>
        /// Gets the resolved service from the unity container.
        /// </summary>
        /// <param name="serviceType">Type of the service to be resolved</param>
        /// <returns>Resolved service</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return unityContainer.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets one or more resolved services from the unity container.
        /// </summary>
        /// <param name="serviceType">Type of the service to be resolved</param>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return unityContainer.ResolveAll(serviceType);
            }
            catch
            {
                return new List<object>();
            }
        }
    }
}
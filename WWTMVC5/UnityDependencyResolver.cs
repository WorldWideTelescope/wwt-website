//-----------------------------------------------------------------------
// <copyright file="UnityDependencyResolver.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace WWTMVC5
{
    /// <summary>
    /// Class representing the unity dependency resolver used for dependency injection.
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver, IServiceProvider
    {
        /// <summary>
        /// Instance of unity container.
        /// </summary>
        private readonly IUnityContainer _unityContainer;

        /// <summary>
        /// Initializes a new instance of the UnityDependencyResolver class.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        public UnityDependencyResolver(IUnityContainer container)
        {
            _unityContainer = container;
        }

        /// <summary>
        /// Gets the resolved service from the unity container.
        /// </summary>
        /// <param name="serviceType">Type of the service to be resolved</param>
        /// <returns>Resolved service</returns>
        object IDependencyResolver.GetService(Type serviceType)
        {
            try
            {
                return _unityContainer.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        object IServiceProvider.GetService(Type serviceType)
            => _unityContainer.Resolve(serviceType);

        /// <summary>
        /// Gets one or more resolved services from the unity container.
        /// </summary>
        /// <param name="serviceType">Type of the service to be resolved</param>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            try
            {
                return _unityContainer.ResolveAll(serviceType);
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}
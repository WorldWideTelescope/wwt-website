using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using WWT.Providers;
using WWTMVC5;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(UnityWebActivator), "Shutdown")]

namespace WWTMVC5
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            var provider = new UnityDependencyResolver(container);

            DependencyResolver.SetResolver(provider);
            RequestProvider.SetServiceProvider(new DependencyResolverServiceProvider(provider));

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            // Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }

        private class DependencyResolverServiceProvider : IServiceProvider
        {
            private readonly IDependencyResolver _resolver;

            public DependencyResolverServiceProvider(IDependencyResolver resolver)
            {
                _resolver = resolver;
            }

            public object GetService(Type serviceType) => _resolver.GetService(serviceType);
        }
    }
}
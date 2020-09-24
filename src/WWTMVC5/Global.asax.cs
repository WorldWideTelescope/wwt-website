using System;
using System.CodeDom;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using WWTMVC5.Models;
using WWTMVC5.Repositories;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services;
using WWTMVC5.Services.Interfaces;
using System.IdentityModel.Services;
using WWT.Providers;
using Microsoft.Practices.Unity.Mvc;
using System.Linq;

namespace WWTMVC5
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RegisterUnityContainer();

            AutoMapperSettings.RegisterControllerAutoMappers();
            AutoMapperSettings.RegisterServiceAutoMappers();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();

            if (error is CryptographicException)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
                SessionWrapper.Clear();
                Server.ClearError();
            }
        }

        /// <summary>
        /// Creates an instance of UnityContainer and registers the instances which needs to be injected
        /// to Controllers/Views/Services, etc.
        /// </summary>
        private static void RegisterUnityContainer()
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            var provider = new UnityDependencyResolver(container);

            DependencyResolver.SetResolver(provider);
            RequestProvider.SetServiceProvider(provider);
        }
    }
}

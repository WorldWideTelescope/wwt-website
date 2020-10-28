﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity.AspNet.Mvc;
using WWT;
using WWT.Azure;
using WWT.Providers;
using WWTWebservices;

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

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            WwtWebHttpHandler.Initialize(BuildServiceProvider());
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IFileNameHasher, Net4x32BitFileNameHasher>();

            services.AddRequestProviders(options =>
            {
                options.DssTerapixelDir = ConfigurationManager.AppSettings["DssTerapixelDir"];
                options.DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"];
                options.DssToastPng = ConfigurationManager.AppSettings["DSSTOASTPNG"];
                options.WWTDEMDir = ConfigurationManager.AppSettings["WWTDEMDir"];
                options.WwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
                options.WwtGalexDir = ConfigurationManager.AppSettings["WWTGALEXDIR"];
                options.WwtToursDBConnectionString = ConfigurationManager.AppSettings["WWTToursDBConnectionString"];
            });

            services
                .AddAzureServices(options =>
                {
                    options.StorageAccount = ConfigurationManager.AppSettings["AzurePlateFileStorageAccount"];
                })
                .AddPlateFiles(options =>
                {
                    options.UseAzurePlateFiles = ConfigReader<bool>.GetSetting("UseAzurePlateFiles");
                })
                .AddThumbnails(options =>
                {
                    options.ContainerName = ConfigurationManager.AppSettings["ThumbnailContainer"];
                    options.Default = ConfigurationManager.AppSettings["DefaultThumbnail"];
                });

            services.AddCaching(options =>
            {
                options.RedisCacheConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
                options.UseCaching = ConfigReader<bool>.GetSetting("UseCaching");
                options.SlidingExpiration = TimeSpan.Parse(ConfigurationManager.AppSettings["SlidingExpiration"]);
            });

            services.AddLogging(builder =>
            {
                builder.AddFilter("Swick.Cache", LogLevel.Trace);
                builder.AddDebug();
            });

            return services.BuildServiceProvider();
        }
    }
}

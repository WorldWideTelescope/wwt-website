using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
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
using WWT.Tours;
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

            services.AddRequestProviders(options =>
            {
                options.DataDir = ConfigurationManager.AppSettings["DataDir"];
                options.DssTerapixelDir = ConfigurationManager.AppSettings["DssTerapixelDir"];
                options.DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"];
                options.DssToastPng = ConfigurationManager.AppSettings["DSSTOASTPNG"];
                options.WWTDEMDir = ConfigurationManager.AppSettings["WWTDEMDir"];
                options.WwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
                options.WwtGalexDir = ConfigurationManager.AppSettings["WWTGALEXDIR"];
                options.WwtToursDBConnectionString = ConfigurationManager.AppSettings["WWTToursDBConnectionString"];
                options.WwtTourCache = ConfigurationManager.AppSettings["WWTTOURCACHE"];

                options.TourVersionCheckIntervalMinutes = int.TryParse(ConfigurationManager.AppSettings["TourVersionCheckIntervalMinutes"], out var min) ? min : 5;
                options.LoginTracking = Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]);
                options.LoggingConn = ConfigurationManager.AppSettings["LoggingConn"];
                options.Webkey = ConfigurationManager.AppSettings["webkey"];
            });

            services
                .AddAzureServices(options =>
                {
                    options.StorageAccount = ConfigurationManager.AppSettings["AzurePlateFileStorageAccount"];
                    options.MarsStorageAccount = ConfigurationManager.AppSettings["MarsStorageAccount"];
                })
                .AddPlateFiles(options =>
                {
                    options.UseAzurePlateFiles = ConfigReader<bool>.GetSetting("UseAzurePlateFiles");
                    options.Container = ConfigurationManager.AppSettings["PlateFileContainer"];
                    options.KnownPlateFile = ConfigurationManager.AppSettings["KnownPlateFile"];
                })
                .AddThumbnails(options =>
                {
                    options.ContainerName = ConfigurationManager.AppSettings["ThumbnailContainer"];
                    options.Default = ConfigurationManager.AppSettings["DefaultThumbnail"];
                })
                .AddCatalog(options =>
                {
                    options.ContainerName = ConfigurationManager.AppSettings["CatalogContainer"];
                })
                .AddTours(options =>
                {
                    options.ContainerName = ConfigurationManager.AppSettings["TourContainer"];
                })
                .AddTiles(options =>
                {
                    options.ContainerName = ConfigurationManager.AppSettings["ImagesTilerContainer"];
                });

            services
                .AddCaching(options =>
                {
                    options.RedisCacheConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
                    options.UseCaching = ConfigReader<bool>.GetSetting("UseCaching");
                    options.SlidingExpiration = TimeSpan.Parse(ConfigurationManager.AppSettings["SlidingExpiration"]);
                })
                .CacheType<IMandelbrot>(m => m.Add(nameof(IMandelbrot.CreateMandelbrot)))
                .CacheType<IVirtualEarthDownloader>(plates => plates.Add(nameof(IVirtualEarthDownloader.DownloadVeTileAsync)))
                .CacheType<IOctTileMapBuilder>(plates => plates.Add(nameof(IOctTileMapBuilder.GetOctTileAsync)))
                .CacheType<IPlateTilePyramid>(plates => plates.Add(nameof(IPlateTilePyramid.GetStreamAsync)))
                .CacheType<IThumbnailAccessor>(plates => plates
                    .Add(nameof(IThumbnailAccessor.GetThumbnailStreamAsync))
                    .Add(nameof(IThumbnailAccessor.GetDefaultThumbnailStreamAsync)))
                .CacheType<ITourAccessor>(plates => plates
                    .Add(nameof(ITourAccessor.GetAuthorThumbnailAsync))
                    .Add(nameof(ITourAccessor.GetTourAsync))
                    .Add(nameof(ITourAccessor.GetTourThumbnailAsync)));

            // Override built-in configuration for ILoggerProvider so everyone uses the same instance
            services.AddSingleton<IOptionsFactory<TelemetryConfiguration>>(new TelemetryConfigurationInstance());

            services.AddLogging(builder =>
            {
                builder.AddFilter("Swick.Cache", LogLevel.Trace);
                builder.AddDebug();

                var appInsightsKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

                if (!string.IsNullOrEmpty(appInsightsKey))
                {
                    builder.AddApplicationInsights(appInsightsKey);
                    builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace);

                    TelemetryConfiguration.Active.InstrumentationKey = appInsightsKey;
                }
            });

            return services.BuildServiceProvider();
        }

        private class TelemetryConfigurationInstance : IOptionsFactory<TelemetryConfiguration>
        {
            public TelemetryConfiguration Create(string name) => TelemetryConfiguration.Active;
        }
    }
}

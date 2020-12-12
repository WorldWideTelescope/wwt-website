#nullable disable

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

using WWT.Azure;
using WWT.Caching;
using WWT.Imaging;
using WWT.PlateFiles;
using WWT.Providers;
using WWT.Tours;

namespace WWT.Web
{
    public partial class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.InstrumentationKey = _configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
            });
            services.AddSingleton<ITelemetryInitializer, ExtraTelemetryInitializer>();
            services.AddApplicationInsightsTelemetryProcessor<WwtTelemetryProcessor>();

            services
             .AddRequestProviders(options =>
             {
                 options.ExternalUrlMapText = _configuration["ExternalUrlMap"];
                 options.WwtToursDBConnectionString = _configuration["WWTToursDBConnectionString"];
             })
             .AddAzureServices(options =>
             {
                 options.StorageAccount = _configuration["AzurePlateFileStorageAccount"];
                 options.MarsStorageAccount = _configuration["MarsStorageAccount"];
             })
             .AddPlateFiles(options =>
             {
                 _configuration.Bind(options);
                 options.Container = _configuration["PlateFileContainer"];
             })
             .AddThumbnails(options =>
             {
                 options.ContainerName = _configuration["ThumbnailContainer"];
                 options.Default = _configuration["DefaultThumbnail"];
             })
             .AddCatalog(options =>
             {
                 options.ContainerName = _configuration["CatalogContainer"];
             })
             .AddTours(options =>
             {
                 options.ContainerName = _configuration["TourContainer"];
             })
             .AddTiles(options =>
             {
                 options.ContainerName = _configuration["ImagesTilerContainer"];
             });

            services
                .AddCaching(options =>
                {
                    _configuration.Bind(options);
                    options.RedisCacheConnectionString = _configuration["RedisConnectionString"];
                    options.SlidingExpiration = TimeSpan.Parse(_configuration["SlidingExpiration"]);
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

            services.AddLogging(builder =>
            {
                builder.AddFilter("Swick.Cache", LogLevel.Trace);
                builder.AddDebug();
            });

            services.AddSnapshotCollector();

            services.AddSingleton(typeof(HelloWorldProvider));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                MapWwtEndpoints(endpoints);
            });
        }

        private static void MapWwtEndpoints(IEndpointRouteBuilder endpoints)
        {
            var cache = new ConcurrentDictionaryCache();
            var endpointManager = endpoints.ServiceProvider.GetRequiredService<EndpointManager>();

            var @public = new CacheControlHeaderValue { Public = true };
            var nocache = new CacheControlHeaderValue { NoCache = true };

            // Some special infrastructure endpoints that don't need to be
            // library-ified:
            //
            // Many web infra health checks assume that your server will return
            // a 200 result for the root path, so let's make sure that actually
            // happens.
            endpointManager.Add("/", typeof(HelloWorldProvider));

            // this URL is requested by the Azure App Service Docker framework
            // to check if the container is running. Azure doesn't care if it
            // 404's, but those 404's do get logged as failures in Application
            // Insights, which we'd like to avoid.
            endpointManager.Add("/robots933456.txt", typeof(HelloWorldProvider));

            foreach (var (endpoint, providerType) in endpointManager)
            {
                endpoints.MapGet(endpoint, ctx =>
                {
                    var provider = (RequestProvider)ctx.RequestServices.GetRequiredService(providerType);

                    ctx.Response.ContentType = provider.ContentType;
                    ctx.Response.GetTypedHeaders().CacheControl = provider.IsCacheable ? @public : nocache;

                    return provider.RunAsync(new AspNetCoreWwtContext(ctx, cache), ctx.RequestAborted);
                });
            }
        }
    }
}

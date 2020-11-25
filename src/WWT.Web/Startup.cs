#nullable disable

using Microsoft.ApplicationInsights.SnapshotCollector;
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
using WWT.Providers;
using WWT.Tours;
using WWTWebservices;

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

            services.AddRequestProviders(options =>
            {

                options.WwtToursDBConnectionString = _configuration["WWTToursDBConnectionString"];
            });

            services
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

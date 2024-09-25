using Microsoft.AspNetCore.Builder;
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

namespace WWT.Web;

public static class WwtStartupExtensions
{
    public static void AddWwt(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.AddKeyedAzureBlobClient("WwtFiles");
        builder.AddKeyedAzureBlobClient("Mars");

        builder.Services
         .AddRequestProviders(options =>
         {
             options.ExternalUrlMapText = configuration["ExternalUrlMap"];
             options.WwtToursDBConnectionString = configuration["WWTToursDBConnectionString"];
         })
         .AddAzureServices(options =>
         {
             options.StorageAccount = configuration["AzurePlateFileStorageAccount"];
             options.MarsStorageAccount = configuration["MarsStorageAccount"];
         })
         .AddPlateFiles(options =>
         {
             configuration.Bind(options);
             options.Container = configuration["PlateFileContainer"];
         })
         .AddThumbnails(options =>
         {
             options.ContainerName = configuration["ThumbnailContainer"];
             options.Default = configuration["DefaultThumbnail"];
         })
         .AddCatalog(options =>
         {
             options.ContainerName = configuration["CatalogContainer"];
         })
         .AddTours(options =>
         {
             options.ContainerName = configuration["TourContainer"];
         })
         .AddTiles(options =>
         {
             options.ContainerName = configuration["ImagesTilerContainer"];
         });

        builder.Services
            .AddCaching(options =>
            {
                configuration.Bind(options);
                options.RedisCacheConnectionString = configuration["RedisConnectionString"];
                options.SlidingExpiration = TimeSpan.Parse(configuration["SlidingExpiration"]);
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

        builder.Services.AddLogging(builder =>
        {
            builder.AddFilter("Swick.Cache", LogLevel.Trace);
            builder.AddDebug();
        });

        builder.Services.AddSingleton(typeof(HelloWorldProvider));
    }

    public static void MapWwt(this IEndpointRouteBuilder endpoints)
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

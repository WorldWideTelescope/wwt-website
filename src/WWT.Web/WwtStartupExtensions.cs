using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using WWT.Azure;
using WWT.Providers;

namespace WWT.Web;

public static class WwtStartupExtensions
{
    private static void MergeConfig(IConfiguration configuration, string oldKey, string newKey)
    {
        if (configuration[oldKey] is { } existing && !string.IsNullOrEmpty(existing))
        {
            configuration[$"ConnectionStrings:{newKey}"] = existing;
        }
    }

    public static void AddWwt(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        // TODO: change the existing configuration to the new expected pattern
        MergeConfig(configuration, "AzurePlateFileStorageAccount", "WwtFiles");
        MergeConfig(configuration, "MarsStorageAccount", "Mars");
        MergeConfig(configuration, "RedisConnectionString", "cache");

        builder.AddKeyedAzureBlobClient("WwtFiles");
        builder.AddKeyedAzureBlobClient("Mars");
        builder.AddRedisDistributedCache("cache");

        builder.Services.AddKeyedSingleton("WWT", new ActivitySource("WWT"));

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

        builder.CacheWwtServices();

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

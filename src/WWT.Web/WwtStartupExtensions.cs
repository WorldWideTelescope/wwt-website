using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Reflection;
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

        builder.Services.AddSingleton<StarChart>();
        builder.AddWwtCaching();
    }

    public static void MapWwt(this IEndpointRouteBuilder endpoints)
    {
        // Many web infra health checks assume that your server will return
        // a 200 result for the root path, so let's make sure that actually
        // happens.
        endpoints.MapGet("/", () =>
        {
            var attr = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            string assemblyVersion = attr?.InformationalVersion ?? "0.0.0-unspecified";
            return TypedResults.Ok($"WWT.Web app version {assemblyVersion}\n");
        });

        // this URL is requested by the Azure App Service Docker framework
        // to check if the container is running. Azure doesn't care if it
        // 404's, but those 404's do get logged as failures in Application
        // Insights, which we'd like to avoid.
        endpoints.MapGet("/robots933456.txt", () => TypedResults.NoContent());

        endpoints.MapWwtProviders();
    }

    private static void MapWwtProviders(this IEndpointRouteBuilder endpoints)
    {
        var cache = new ConcurrentDictionaryCache();
        var endpointManager = endpoints.ServiceProvider.GetRequiredService<EndpointManager>();

        var @public = new CacheControlHeaderValue { Public = true };
        var nocache = new CacheControlHeaderValue { NoCache = true };

        foreach (var (endpoint, providerType) in endpointManager)
        {
            // All the providers are registered as singletons, so we can cache them initially
            var provider = (RequestProvider)ActivatorUtilities.CreateInstance(endpoints.ServiceProvider, providerType);
            endpoints.MapGet(endpoint, (HttpContext ctx, [FromKeyedServices("WWT")] ActivitySource activitySource) =>
            {
                using var activity = activitySource.StartActivity("RequestProvider", ActivityKind.Server);
                activity?.AddBaggage("ProviderName", providerType.FullName);

                ctx.Response.ContentType = provider.ContentType;
                ctx.Response.GetTypedHeaders().CacheControl = provider.IsCacheable ? @public : nocache;

                return provider.RunAsync(new AspNetCoreWwtContext(ctx, cache), ctx.RequestAborted);
            });
        }
    }
}

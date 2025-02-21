using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IO;
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

        // Redis doesn't register this but it does implement it, so we'll force its registration here
        builder.Services.AddSingleton(sp => (IBufferDistributedCache)sp.GetRequiredService<IDistributedCache>());

        builder.Services.AddSingleton<RecyclableMemoryStreamManager>();
        builder.Services.AddKeyedSingleton(Constants.ActivitySourceName, new ActivitySource(Constants.ActivitySourceName));
        builder.Services.AddSingleton<SDSSToastProvider>();
        builder.Services.AddSingleton<DSSProvider>();
        builder.Services.AddSingleton<MarsHiriseProvider>();

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
    }
}

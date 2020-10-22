using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using WWTWebservices;

namespace WWT.Azure
{
    public static class AzureWwtExtensions
    {
        public static AzureServiceBuilder AddAzureServices(this IServiceCollection services, Action<AzureOptions> configure)
        {
            var azureOptions = new AzureOptions();
            configure(azureOptions);

            if (Uri.TryCreate(azureOptions.StorageAccount, UriKind.Absolute, out var storageUri))
            {
                services.AddSingleton<TokenCredential, DefaultAzureCredential>();
                services.AddTransient(ctx => new BlobServiceClient(storageUri, ctx.GetRequiredService<TokenCredential>()));
            }
            else
            {
                services.AddTransient(_ => new BlobServiceClient(azureOptions.StorageAccount));
            }

            return new AzureServiceBuilder(services);
        }

        public static AzureServiceBuilder AddThumbnails(this AzureServiceBuilder services, Action<ThumbnailOptions> configure)
        {
            var thumbnailOptions = new ThumbnailOptions();
            configure(thumbnailOptions);

            services.Services.AddSingleton(thumbnailOptions);
            services.Services.AddSingleton<IThumbnailAccessor, AzureThumbnailAccessor>();

            return services;
        }

        public static AzureServiceBuilder AddPlateFiles(this AzureServiceBuilder services, Action<AzurePlateTilePyramidOptions> configure)
        {
            var plateTileOptions = new AzurePlateTilePyramidOptions();
            configure(plateTileOptions);

            services.Services.AddSingleton(plateTileOptions);

            if (plateTileOptions.UseAzurePlateFiles)
            {
                services.Services.AddSingleton<IPlateTilePyramid, SeekableAzurePlateTilePyramid>();
            }
            else
            {
                services.Services.AddSingleton<IPlateTilePyramid, FilePlateTilePyramid>();
            }

            return services;
        }

        public class AzureServiceBuilder
        {
            public AzureServiceBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }
        }
    }
}
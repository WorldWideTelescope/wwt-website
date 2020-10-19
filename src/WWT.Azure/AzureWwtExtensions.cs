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
        public static void AddAzureServices(this IServiceCollection services, Action<AzurePlateTilePyramidOptions> configure)
        {
            var azureOptions = new AzurePlateTilePyramidOptions();

            configure(azureOptions);

            services.AddSingleton(azureOptions);

            if (azureOptions.UseAzurePlateFiles)
            {
                if (Uri.TryCreate(azureOptions.StorageAccount, UriKind.Absolute, out var storageUri))
                {
                    services.AddSingleton<TokenCredential, DefaultAzureCredential>();
                    services.AddTransient(ctx => new BlobServiceClient(storageUri, ctx.GetRequiredService<TokenCredential>()));
                }
                else
                {
                    services.AddTransient(_ => new BlobServiceClient(azureOptions.StorageAccount));
                }

                services.AddSingleton<IPlateTilePyramid, SeekableAzurePlateTilePyramid>();
            }
            else
            {
                services.AddSingleton<IPlateTilePyramid, FilePlateTilePyramid>();
            }
        }
    }
}
using Aspire.Hosting.Azure;
using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace WWT.AppHost;

#pragma warning disable ASPIREEVENTING001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal static class DataSeedExtensions
{
    /// <summary>
    /// Initialize the blob with development data from the WWT data app.
    /// </summary>
    public static IResourceBuilder<AzureBlobStorageResource> WithDevData(this IResourceBuilder<AzureBlobStorageResource> blob)
    {
        var seedData = blob.ApplicationBuilder.AddResource(new SeedDataResource($"{blob.Resource.Name}-data"));

        blob.ApplicationBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IDistributedApplicationLifecycleHook, DataSeedLifecycleHood>());
        blob.ApplicationBuilder.Services.AddHttpClient(seedData.Resource.Name);

        blob.ApplicationBuilder.Eventing.Subscribe<BeforeResourceStartedEvent>(seedData.Resource, async (e, token) =>
        {
            var rns = e.Services.GetRequiredService<ResourceNotificationService>();
            var rls = e.Services.GetRequiredService<ResourceLoggerService>();

            var logger = rls.GetLogger(seedData.Resource);

            await rns.PublishUpdateAsync(seedData.Resource, s => s with { State = "Preparing data" });

            using var client = e.Services.GetRequiredService<IHttpClientFactory>().CreateClient(e.Resource.Name);

            try
            {
                var downloadTask = DataManager.Create(e, logger, client, token);
                var resource = rns.WaitForResourceAsync(blob.Resource.Name, KnownResourceStates.Running, token);

                await Task.WhenAll(downloadTask, resource);

                var download = await downloadTask;

                await download.UploadAsync(blob.Resource, client, token);

                await rns.PublishUpdateAsync(seedData.Resource, s => s with { State = KnownResourceStates.Finished });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed data resource");
                await rns.PublishUpdateAsync(seedData.Resource, s => s with { State = KnownResourceStates.FailedToStart });
            }
        });

        return blob;
    }

    private sealed class DataSeedLifecycleHood(IServiceProvider services, IDistributedApplicationEventing events) : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var seed in appModel.Resources.OfType<SeedDataResource>())
            {
                await events.PublishAsync(new BeforeResourceStartedEvent(seed, services), cancellationToken);
            };
        }
    }

    private sealed class DataManager(ILogger logger, string path)
    {
        public static async Task<DataManager> Create(BeforeResourceStartedEvent b, ILogger logger, HttpClient client, CancellationToken token)
        {
            var path = Path.Combine(Path.GetTempPath(), "wwt-website", "dev-data.zip");

            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                await DownloadAsync();
            }
            else
            {
                logger.LogInformation("Using cached dev data at {Path}", path);
            }

            return new DataManager(logger, path);

            async Task DownloadAsync()
            {
                const string url = "https://wwtcoreapp-data-app.azurewebsites.net/v2/data/dev_export";

                logger.LogInformation("Downloading dev data from {Url}", url);

                using var result = await client.GetStreamAsync(url, token);

                using var fs = File.OpenWrite(path);
                await result.CopyToAsync(fs, token);
            }
        }

        public async Task UploadAsync(AzureBlobStorageResource resource, HttpClient httpClient, CancellationToken token)
        {
            var blobClient = new BlobServiceClient(resource.ConnectionStringExpression.ValueExpression, new() { Transport = new HttpClientTransport(httpClient) });

            using var fs = File.OpenRead(path);
            using var archive = new ZipArchive(fs, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                var idx = entry.FullName.IndexOf('/');

                var containerName = entry.FullName[..idx];
                var blobName = entry.FullName[(idx + 1)..];

                logger.LogInformation("Uploading {Name} to {Container}", blobName, containerName);

                var container = blobClient.GetBlobContainerClient(containerName);

                await container.CreateIfNotExistsAsync(cancellationToken: token);

                var blob = container.GetBlobClient(blobName);

                using var archiveStream = entry.Open();
                await blob.UploadAsync(archiveStream, token);
                logger.LogInformation("Uploaded {Name} to {Container}", blobName, containerName);
            }
        }
    }
}

internal class SeedDataResource(string name) : Resource(name)
{
}

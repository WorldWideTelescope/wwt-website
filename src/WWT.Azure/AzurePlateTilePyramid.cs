using Azure.Storage.Blobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Azure
{
    /// <summary>
    /// Manages the interface to Azure storage for the decomposition of plate files
    /// Note that the presence of the "tag" parameter is an indicator of the PlateTile2 format in use
    /// </summary>
    public class AzurePlateTilePyramid : IPlateTilePyramid
    {
        private readonly AzurePlateTilePyramidOptions _options;
        private readonly BlobServiceClient _service;
        private readonly ConcurrentDictionary<string, Task<BlobContainerClient>> _containers;

        private readonly Dictionary<string, (string container, string blob)> _plateNameMapping = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            { "dssterrapixel.plate", ("dss", "DSSTerraPixelL{0}X{1}Y{2}.png") }
        };

        public AzurePlateTilePyramid(AzurePlateTilePyramidOptions options, BlobServiceClient service)
        {
            _options = options;
            _service = service;
            _containers = new ConcurrentDictionary<string, Task<BlobContainerClient>>();
        }

        public async Task SaveStreamAsync(Stream stream, string plateName, string fileName, CancellationToken token)
        {
            var container = await GetBlobContainerClientAsync(plateName).ConfigureAwait(false);
            var client = container.GetBlobClient(fileName);

            await client.UploadAsync(stream, _options.OverwriteExisting, token);
        }

        public async Task SaveStreamAsync(Stream stream, string plateName, int level, int x, int y, CancellationToken token)
        {
            var client = await GetBlobClientAsync(plateName, level, x, y).ConfigureAwait(false);

            await client.UploadAsync(stream, _options.OverwriteExisting, token);
        }

        /// <summary>
        /// Saves a PlateFile2 stream to blob storage
        /// </summary>
        public async Task SaveStreamAsync(Stream stream, string plateName, int tag, int level, int x, int y, CancellationToken token)
        {
            var client = await GetBlobClientAsync(plateName, tag, level, x, y).ConfigureAwait(false);

            await client.UploadAsync(stream, _options.OverwriteExisting, token);
        }

        Stream IPlateTilePyramid.GetStream(string pathPrefix, string plateName, int level, int x, int y)
            => GetStream(plateName, level, x, y);

        public Stream GetStream(string plateName, int level, int x, int y)
        {
            var client = GetBlobClientAsync(plateName, level, x, y);
            var download = client.Result.Download();

            return download.Value.Content;
        }

        public Stream GetStream(string pathPrefix, string plateName, int tag, int level, int x, int y)
        {
            var client = GetBlobClientAsync(plateName, tag, level, x, y);
            var download = client.Result.Download();
            return download.Value.Content;
        }

        private Task<BlobContainerClient> GetBlobContainerClientAsync(string plateName)
            => _containers.GetOrAdd(plateName, async p =>
            {
                var name = GetBlobContainerName(p);
                var client = _service.GetBlobContainerClient(name);

                if (_options.CreateContainer)
                {
                    await client.CreateIfNotExistsAsync();
                }

                return client;
            });

        private async Task<BlobClient> GetBlobClientAsync(string plateName, int level, int x, int y)
        {
            var container = await GetBlobContainerClientAsync(plateName).ConfigureAwait(false);
            var blobName = GetBlobName(plateName, level, x, y);

            return container.GetBlobClient(blobName);
        }

        private async Task<BlobClient> GetBlobClientAsync(string plateName, int tag, int level, int x, int y)
        {
            var container = await GetBlobContainerClientAsync(plateName).ConfigureAwait(false);
            var blobName = GetBlobName(tag, level, x, y);
            return container.GetBlobClient(blobName);
        }

        private string GetBlobContainerName(string plateName)
        {
            if (_plateNameMapping.TryGetValue(plateName, out var info))
            {
                return info.container;
            }

            return Path.GetFileNameWithoutExtension(plateName).ToLowerInvariant();
        }

        private string GetBlobName(string plateName, int level, int x, int y)
        {
            var blobFormat = "L{0}X{1}Y{2}.png";

            if (_plateNameMapping.TryGetValue(plateName, out var info))
            {
                blobFormat = info.blob;
            }

            return string.Format(blobFormat, level, x, y);
        }

        /// <summary>
        /// Gets the URL upload pattern for a blob for a PlateFile2 image
        /// </summary>
        private static string GetBlobName(int tag, int level, int x, int y) 
            => $"{tag}/L{level}X{x}Y{y}.png";

    }
}

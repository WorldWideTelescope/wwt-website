using Azure.Storage.Blobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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

        public Task<bool> SaveStreamAsync(Stream stream, string plateName, CancellationToken token)
            => SaveStreamAsync(stream, plateName, plateName, token);

        public async Task<bool> SaveStreamAsync(Stream stream, string plateName, string fileName, CancellationToken token)
        {
            var container = await GetBlobContainerClientAsync(plateName).ConfigureAwait(false);
            var client = container.GetBlobClient(fileName);

            return await SaveStreamAsync(client, stream, token).ConfigureAwait(false);
        }

        public async Task<bool> SaveStreamAsync(Stream stream, string plateName, int level, int x, int y, CancellationToken token)
        {
            var client = await GetBlobClientAsync(plateName, level, x, y).ConfigureAwait(false);

            return await SaveStreamAsync(client, stream, token).ConfigureAwait(false);
        }

        private async Task<bool> SaveStreamAsync(BlobClient client, Stream stream, CancellationToken token)
        {
            if (_options.SkipIfExists && await client.ExistsAsync(token).ConfigureAwait(false))
            {
                return false;
            }

            await client.UploadAsync(stream, _options.OverwriteExisting, token).ConfigureAwait(false);

            return true;
        }

        Task<Stream> IPlateTilePyramid.GetStreamAsync(string pathPrefix, string plateName, int level, int x, int y, CancellationToken token)
            => GetStreamAsync(plateName, level, x, y, token);

        public async Task<Stream> GetStreamAsync(string plateName, int level, int x, int y, CancellationToken token)
        {
            var client = await GetBlobClientAsync(plateName, level, x, y).ConfigureAwait(false);
            var download = client.Download();

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

        private string GetBlobContainerName(string plateName)
        {
            if (_plateNameMapping.TryGetValue(plateName, out var info))
            {
                return info.container;
            }

            return _options.Container;
        }

        /// <summary>
        /// Azure Blob storage uses a virtual "folder" structure - the blob name having /'s indicates folders
        /// Ex: coredata/testname/0/test.png
        ///     container  blobName, but will show as folder testname, subfolder 0 with test.png in it
        /// </summary>
        private string GetBlobName(string plateName, int level, int x, int y)
        {
            if (_plateNameMapping.TryGetValue(plateName, out var info))
            {
                return string.Format(info.blob, level, x, y);
            }
            var blobName = $"{Path.GetFileNameWithoutExtension(plateName).ToLowerInvariant()}/L{level}X{x}Y{y}.png";
            return blobName;
        }

        public async IAsyncEnumerable<string> GetPlateNames([EnumeratorCancellation] CancellationToken token)
        {
            foreach (var names in _plateNameMapping)
            {
                yield return names.Key;
            }

            var container = _service.GetBlobContainerClient(_options.Container);

            await foreach (var item in container.GetBlobsByHierarchyAsync(delimiter: "/", cancellationToken: token).WithCancellation(token))
            {
                var prefix = item.Prefix.TrimEnd('/');
                yield return $"{prefix}.plate";
            }
        }

        public async Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int tag, int level, int x, int y, CancellationToken token)
        {
            var container = await GetBlobContainerClientAsync(plateName).ConfigureAwait(false);
            var client = container.GetBlobClient(plateName);
            var stream = await client.OpenReadAsync();

            return PlateFile2.GetImageStream(stream, tag, level, x, y);
        }
    }
}
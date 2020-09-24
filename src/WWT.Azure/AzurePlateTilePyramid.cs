using Azure.Core;
using Azure.Storage.Blobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using WWTWebservices;

namespace WWT.Azure
{
    public class AzurePlateTilePyramid : IPlateTilePyramid
    {
        private readonly AzurePlateTilePyramidOptions _options;
        private readonly BlobServiceClient _service;
        private readonly ConcurrentDictionary<string, BlobContainerClient> _containers;

        private readonly Dictionary<string, (string container, string blob)> _plateNameMapping = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            { "dssterrapixel.plate", ("dss", "DSSTerraPixelL{0}X{1}Y{2}.png") }
        };

        public AzurePlateTilePyramid(AzurePlateTilePyramidOptions options, TokenCredential credentials)
        {
            _options = options;
            _service = new BlobServiceClient(new Uri(options.StorageUri), credentials);
            _containers = new ConcurrentDictionary<string, BlobContainerClient>();
        }

        public void SaveStream(Stream stream, string plateName, string fileName)
        {
            var container = GetBlobContainerClient(plateName);
            var client = container.GetBlobClient(fileName);

            client.Upload(stream, _options.OverwriteExisting);
        }

        public void SaveStream(Stream stream, string plateName, int level, int x, int y)
        {
            var client = GetBlobClient(plateName, level, x, y);

            client.Upload(stream, _options.OverwriteExisting);
        }

        Stream IPlateTilePyramid.GetStream(string pathPrefix, string plateName, int level, int x, int y)
            => GetStream(plateName, level, x, y);

        public Stream GetStream(string plateName, int level, int x, int y)
        {
            var client = GetBlobClient(plateName, level, x, y);
            var download = client.Download();

            return download.Value.Content;
        }

        private BlobContainerClient GetBlobContainerClient(string plateName)
            => _containers.GetOrAdd(plateName, p =>
            {
                var name = GetBlobContainerName(p);
                var client = _service.GetBlobContainerClient(name);

                if (_options.CreateContainer)
                {
                    client.CreateIfNotExists();
                }

                return client;
            });

        private BlobClient GetBlobClient(string plateName, int level, int x, int y)
        {
            var container = GetBlobContainerClient(plateName);
            var blobName = GetBlobName(plateName, level, x, y);

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
    }
}

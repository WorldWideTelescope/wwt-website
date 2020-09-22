using Azure.Core;
using Azure.Storage.Blobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace WWTWebservices.Azure
{
    public class AzurePlateTilePyramid : IPlateTilePyramid
    {
        private readonly Dictionary<string, (string container, string blob)> _plateNameMapping = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            { "dssterrapixel.plate", ("dss", "DSSTerraPixelL{0}X{1}Y{2}.png") }
        };

        private readonly BlobServiceClient _service;
        private readonly ConcurrentDictionary<string, BlobContainerClient> _containers;

        public AzurePlateTilePyramid(string storageUri, TokenCredential credentials)
        {
            _service = new BlobServiceClient(new Uri(storageUri), credentials);
            _containers = new ConcurrentDictionary<string, BlobContainerClient>();
        }

        public Stream GetStream(string pathPrefix, string plateName, int level, int x, int y)
        {
            var container = _containers.GetOrAdd(plateName, p =>
            {
                var name = GetBlobContainerName(plateName);

                return _service.GetBlobContainerClient(name);
            });

            var blobName = GetBlobName(plateName, level, x, y);
            var client = container.GetBlobClient(blobName);
            var download = client.Download();

            return download.Value.Content;
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

using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using WWTWebservices;

namespace WWT.Azure
{
    public class SeekableAzurePlateTilePyramid : IPlateTilePyramid
    {
        private readonly Func<string, BlobClient> _blobRetriever;
        private readonly BlobContainerClient _container;
        private readonly ILogger<SeekableAzurePlateTilePyramid> _logger;

        public SeekableAzurePlateTilePyramid(AzurePlateTilePyramidOptions options, BlobServiceClient service, ILogger<SeekableAzurePlateTilePyramid> logger)
        {
            _container = service.GetBlobContainerClient(options.Container);
            _logger = logger;

            var cache = new ConcurrentDictionary<string, BlobClient>();
            _blobRetriever = plateName => cache.GetOrAdd(plateName, _container.GetBlobClient);
        }

        Stream IPlateTilePyramid.GetStream(string pathPrefix, string plateName, int level, int x, int y)
            => GetStream(plateName, level, x, y);

        public Stream GetStream(string plateName, int level, int x, int y)
        {
            var client = _blobRetriever(plateName);

            try
            {
                var download = client.OpenRead();

                return PlateTilePyramid.GetImageStream(download, level, x, y);
            }
            catch (RequestFailedException e)
            {
                _logger.LogError(e, "Unexpected error downloading {PlateName}", plateName);
                return null;
            }
        }

        public async IAsyncEnumerable<string> GetPlateNames([EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var item in _container.GetBlobsByHierarchyAsync(delimiter: "/", cancellationToken: token))
            {
                var prefix = item.Prefix.TrimEnd('/');
                yield return $"{prefix}.plate";
            }
        }

        Stream IPlateTilePyramid.GetStream(string pathPrefix, string plateName, int tag, int level, int x, int y)
            => GetStream(plateName, tag, level, x, y);

        public Stream GetStream(string plateName, int tag, int level, int x, int y)
        {
            var client = _blobRetriever(plateName);

            try
            {
                var stream = client.OpenRead();

                return PlateFile2.GetImageStream(stream, tag, level, x, y);
            }
            catch (RequestFailedException e)
            {
                _logger.LogError(e, "Unexpected error downloading {PlateName}", plateName);
                return null;
            }
        }
    }
}
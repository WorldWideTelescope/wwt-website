using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Azure
{
    public class MarsMolaAwareSeekableAzurePlateTilePyramid : MarsAwareSeekableAzurePlateTilePyramid
    {
        private readonly BlobContainerClient _container;

        public MarsMolaAwareSeekableAzurePlateTilePyramid(
            AzurePlateTilePyramidOptions options,
            AzureServiceAccessor services,
            ILogger<SeekableAzurePlateTilePyramid> logger)
            : base(options, services, logger)
        {
            _container = services.WwtFiles.GetBlobContainerClient("marsmola");
        }

        public override Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int level, int x, int y, CancellationToken token)
        {
            if (string.Equals(plateName, "marsmola.plate", StringComparison.Ordinal))
            {
                return GetMarsMolaStream(level, x, y, token);
            }
            else
            {
                return base.GetStreamAsync(pathPrefix, plateName, level, x, y, token);
            }
        }

        private async Task<Stream> GetMarsMolaStream(int level, int x, int y, CancellationToken token)
        {
            var blob = _container.GetBlobClient($"marsmolaL{level}X{x}Y{y}.png");

            try
            {
                return await blob.OpenReadAsync(_readOptions, token).ConfigureAwait(false);
            }
            catch (RequestFailedException e)
            {
                _logger.LogError(e, "Error getting MarsMola stream");
                return null;
            }
        }
    }
}
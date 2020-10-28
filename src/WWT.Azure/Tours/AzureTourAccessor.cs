using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Tours;

namespace WWT.Azure
{
    public class AzureTourAccessor : ITourAccessor
    {
        private readonly BlobContainerClient _container;
        private readonly BlobOpenReadOptions _readOptions;
        private readonly ILogger<AzureTourAccessor> _logger;

        public AzureTourAccessor(AzureTourOptions options, BlobServiceClient service, ILogger<AzureTourAccessor> logger)
        {
            _container = service.GetBlobContainerClient(options.ContainerName);
            _readOptions = new BlobOpenReadOptions(allowModifications: false);
            _logger = logger;
        }

        public Task<Stream> GetAuthorThumbnailAsync(string id, CancellationToken token)
           => GetStream($"{id}_AuthorThumb.bin", token);

        public Task<Stream> GetTourAsync(string id, CancellationToken token)
           => GetStream($"{id}.bin", token);

        public Task<Stream> GetTourThumbnailAsync(string id, CancellationToken token)
           => GetStream($"{id}_TourThumb.bin", token);

        private async Task<Stream> GetStream(string name, CancellationToken token)
        {
            var blob = _container.GetBlobClient(name);

            try
            {
                return await blob.OpenReadAsync(_readOptions, token).ConfigureAwait(false);
            }
            catch (RequestFailedException e)
            {
                _logger.LogDebug(e, "Could not open blob {Name} for reading", name);
                return null;
            }
        }
    }
}
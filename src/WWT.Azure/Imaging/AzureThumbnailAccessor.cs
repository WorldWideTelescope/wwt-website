#nullable disable

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Azure
{
    public class AzureThumbnailAccessor : IThumbnailAccessor
    {
        private readonly ThumbnailOptions _options;
        private readonly BlobContainerClient _container;
        private readonly BlobOpenReadOptions _blobOpenReadOptions;

        public AzureThumbnailAccessor(ThumbnailOptions options, BlobServiceClient service)
        {
            _options = options;
            _container = service.GetBlobContainerClient(options.ContainerName);
            _blobOpenReadOptions = new BlobOpenReadOptions(allowModifications: false);
        }

        public Task<Stream> GetDefaultThumbnailStreamAsync(CancellationToken token)
            => GetThumbnailStreamFromFileAsync(_options.Default, "fromAssembly", token).AsTask();

        public async Task<Stream> GetThumbnailStreamAsync(string name, string type, CancellationToken token)
            => await GetThumbnailStreamAsync(name?.ToLowerInvariant(), token)
            ?? await GetThumbnailStreamAsync(type?.ToLowerInvariant(), token);

        private async ValueTask<Stream> GetThumbnailStreamAsync(string fileName, CancellationToken token)
            => await GetThumbnailStreamFromFileAsync(fileName, "fromAssembly", token) 
            ?? await GetThumbnailStreamFromFileAsync(fileName, "fromBackup", token);

        private async ValueTask<Stream> GetThumbnailStreamFromFileAsync(string fileName, string sub, CancellationToken token)
        {
            if (fileName is null)
            {
                return null;
            }

            var blob = _container.GetBlobClient($"{sub}/{fileName}.jpg");

            if (blob is null || !await blob.ExistsAsync(token).ConfigureAwait(false))
            {
                return null;
            }

            return await blob.OpenReadAsync(_blobOpenReadOptions).ConfigureAwait(false);
        }
    }
}

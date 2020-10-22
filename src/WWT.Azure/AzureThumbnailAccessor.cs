using Azure.Storage.Blobs;
using System.IO;

namespace WWT.Azure
{
    public class AzureThumbnailAccessor : IThumbnailAccessor
    {
        private readonly ThumbnailOptions _options;
        private readonly BlobContainerClient _container;

        public AzureThumbnailAccessor(ThumbnailOptions options, BlobServiceClient service)
        {
            _options = options;
            _container = service.GetBlobContainerClient("thumbnails");
        }

        public Stream GetThumbnailStream(string name, string type)
            => GetThumbnailStream(name) ?? GetThumbnailStream(type) ?? GetThumbnailStream(_options.Default);

        private Stream GetThumbnailStream(string fileName)
            => GetThumbnailStreamFromFile(fileName, "fromAssembly") ?? GetThumbnailStreamFromFile(fileName, "fromBackup");

        private Stream GetThumbnailStreamFromFile(string fileName, string sub)
        {
            if (fileName is null)
            {
                return null;
            }

            var blob = _container.GetBlobClient($"{sub}/{fileName}.jpg");

            if (blob.Exists())
            {
                return blob.OpenRead();
            }

            return null;
        }
    }
}
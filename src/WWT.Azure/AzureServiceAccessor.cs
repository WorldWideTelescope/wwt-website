#nullable disable

using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

namespace WWT.Azure
{
    public class AzureServiceAccessor
    {
        public AzureServiceAccessor([FromKeyedServices("WwtFiles")] BlobServiceClient wwtFiles, [FromKeyedServices("Mars")] BlobServiceClient mars)
        {
            WwtFiles = wwtFiles;
            Mars = mars;
        }

        public BlobServiceClient WwtFiles { get; set; }

        public BlobServiceClient Mars { get; set; }
    }
}

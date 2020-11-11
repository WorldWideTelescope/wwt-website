using Azure.Core;
using Azure.Storage.Blobs;
using System;

namespace WWT.Azure
{
    public class AzureServiceAccessor
    {
        public AzureServiceAccessor(AzureOptions options, TokenCredential credential)
        {
            WwtFiles = CreateServiceClient(options.StorageAccount, credential);
            Mars = CreateServiceClient(options.MarsStorageAccount, credential) ?? WwtFiles;
        }

        public BlobServiceClient WwtFiles { get; set; }

        public BlobServiceClient Mars { get; set; }

        /// <summary>
        /// Creates a <see cref="BlobServiceClient"/> given a string. If the string is a URI, it is assumed to
        /// be useable via token (ie Managed Identity). Otherwise, it is assumed to be a connection string.
        /// </summary>
        private static BlobServiceClient CreateServiceClient(string storageAccount, TokenCredential credential)
        {
            if (storageAccount is null)
            {
                return null;
            }

            if (Uri.TryCreate(storageAccount, UriKind.Absolute, out var storageUri))
            {
                return new BlobServiceClient(storageUri, credential);
            }
            else
            {
                return new BlobServiceClient(storageAccount);
            }
        }
    }
}
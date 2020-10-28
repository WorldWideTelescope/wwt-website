using Autofac;
using AutofacContrib.NSubstitute;
using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace WWT.Azure.Tests
{
    public class AzureTourFileTests
    {
        private readonly Fixture _fixture;

        public AzureTourFileTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAuthorThumbnailTests(bool exists)
        {
            // Arrange
            var id = _fixture.Create<string>();
            var mock = CreateMock($"{id}_AuthorThumb.bin", exists);

            // Act
            var result = await mock.Resolve<AzureTourAccessor>().GetAuthorThumbnailAsync(id, default);

            // Assert
            mock.Resolve<BlobContainerClient>().Received(1).GetBlobClient($"{id}_AuthorThumb.bin");

            if (exists)
            {
                Assert.Same(mock.Resolve<DownloadResult>(), result);
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetTourThumbnailTests(bool exists)
        {
            // Arrange
            var id = _fixture.Create<string>();
            var mock = CreateMock($"{id}_TourThumb.bin", exists);

            // Act
            var result = await mock.Resolve<AzureTourAccessor>().GetTourThumbnailAsync(id, default);

            // Assert
            mock.Resolve<BlobContainerClient>().Received(1).GetBlobClient($"{id}_TourThumb.bin");

            if (exists)
            {
                Assert.Same(mock.Resolve<DownloadResult>(), result);
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetTourTests(bool exists)
        {
            // Arrange
            var id = _fixture.Create<string>();
            var mock = CreateMock($"{id}.bin", exists);

            // Act
            var result = await mock.Resolve<AzureTourAccessor>().GetTourAsync(id, default);

            // Assert
            mock.Resolve<BlobContainerClient>().Received(1).GetBlobClient($"{id}.bin");

            if (exists)
            {
                Assert.Same(mock.Resolve<DownloadResult>(), result);
            }
            else
            {
                Assert.Null(result);
            }
        }

        private IContainer CreateMock(string name, bool exists)
        {
            var options = _fixture.Create<AzureTourOptions>();

            return AutoSubstitute.Configure()
              .InjectProperties()
              .MakeUnregisteredTypesPerLifetime()
              .Provide(options)
              .SubstituteFor<DownloadResult>()
              .SubstituteFor<BlobClient>()
                  .ConfigureSubstitute((client, ctx) =>
                  {
                      if (exists)
                      {
                          var s = ctx.Resolve<DownloadResult>();
                          client.Configure().OpenReadAsync(Arg.Any<BlobOpenReadOptions>()).Returns(s);
                      }
                      else
                      {
                          Task<Stream> Throws(CallInfo _) => throw new RequestFailedException(string.Empty);

                          client.Configure().OpenReadAsync(Arg.Any<BlobOpenReadOptions>()).Returns(Throws);
                      }
                  })
              .SubstituteFor<BlobContainerClient>()
                  .ResolveReturnValue(container => container.GetBlobClient(name))
              .SubstituteFor<BlobServiceClient>()
                  .ResolveReturnValue(service => service.GetBlobContainerClient(options.ContainerName))
              .Build().Container;
        }
    }
}

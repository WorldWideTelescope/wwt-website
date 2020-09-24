using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NSubstitute;
using NSubstitute.Extensions;
using System.IO;
using System.Threading;
using Xunit;

namespace WWT.Azure.Tests
{
    public class AzurePlateTilePyramidTests
    {
        private readonly Fixture _fixture;

        public AzurePlateTilePyramidTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData("test.plate", 0, 0, 0, null, null)]
        [InlineData("dssterrapixel.plate", 0, 0, 0, "dss", "DSSTerraPixelL{0}X{1}Y{2}.png")]
        public void GetStreamTests(string plateFile, int level, int x, int y, string containerName, string blobFormat)
        {
            // Arrange
            var (service, container, _, result) = ConfigureServiceClient(plateFile, level, x, y, containerName, blobFormat);
            var pyramid = new AzurePlateTilePyramid(new AzurePlateTilePyramidOptions(), service);

            // Act
            using var stream = pyramid.GetStream(plateFile, level, x, y);

            // Assert
            Assert.NotNull(stream);
            Assert.Same(result, stream);

            container.DidNotReceive().CreateIfNotExists();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateContainer(bool isCreateCalled)
        {
            // Arrange
            var plateFile = _fixture.Create<string>();
            var level = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var (service, container, _, _) = ConfigureServiceClient(plateFile, level, x, y);
            var options = new AzurePlateTilePyramidOptions
            {
                CreateContainer = isCreateCalled
            };
            var pyramid = new AzurePlateTilePyramid(options, service);

            // Act
            using var stream1 = pyramid.GetStream(plateFile, level, x, y);
            using var stream2 = pyramid.GetStream(plateFile, level, x, y);

            // Assert
            Assert.Same(stream1, stream2);

            if (isCreateCalled)
            {
                container.Received(1).CreateIfNotExists();
            }
            else
            {
                container.DidNotReceive().CreateIfNotExists();
            }
        }

        [Theory]
        [InlineData("test.plate", 0, 0, 0, true, null, null)]
        [InlineData("test.plate", 0, 0, 0, false, null, null)]
        [InlineData("dssterrapixel.plate", 0, 0, 0, true, "dss", "DSSTerraPixelL{0}X{1}Y{2}.png")]
        [InlineData("dssterrapixel.plate", 0, 0, 0, false, "dss", "DSSTerraPixelL{0}X{1}Y{2}.png")]
        public void SaveStreamTests(string plateFile, int level, int x, int y, bool overwrite, string containerName, string blobFormat)
        {
            // Arrange
            var (service, _, blob, _) = ConfigureServiceClient(plateFile, level, x, y, containerName, blobFormat);
            var options = new AzurePlateTilePyramidOptions
            {
                OverwriteExisting = overwrite
            };
            var stream = Substitute.ForPartsOf<Stream>();
            var pyramid = new AzurePlateTilePyramid(options, service);

            // Act
            pyramid.SaveStream(stream, plateFile, level, x, y);

            // Assert
            blob.Received(1).Upload(stream, overwrite);
        }

        private static (BlobServiceClient, BlobContainerClient, BlobClient, Stream) ConfigureServiceClient(string plateFile, int level, int x, int y, string expectedContainerName = null, string blobFormat = null)
        {
            blobFormat ??= "L{0}X{1}Y{2}.png";
            var blobName = string.Format(blobFormat, level, x, y);
            var containerName = expectedContainerName ?? plateFile.Replace(".plate", string.Empty);
            var result = Substitute.ForPartsOf<Stream>();

            var service = Substitute.ForPartsOf<BlobServiceClient>();
            var container = Substitute.ForPartsOf<BlobContainerClient>();
            var blob = Substitute.ForPartsOf<BlobClient>();
            var response = Substitute.ForPartsOf<Response<BlobDownloadInfo>>();

            service.Configure()
                    .GetBlobContainerClient(containerName).Returns(container);

            container.Configure()
                    .GetBlobClient(blobName).Returns(blob);
            container.Configure()
                .CreateIfNotExists().Returns(Substitute.For<Response<BlobContainerInfo>>());

            response.Value.Returns(BlobsModelFactory.BlobDownloadInfo(content: result));

            blob.Configure()
                .Download().Returns(response);

            blob.WhenForAnyArgs(b => b.Upload(Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()))
                .DoNotCallBase();

            return (service, container, blob, result);
        }
    }
}

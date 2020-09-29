using AutofacContrib.NSubstitute;
using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NSubstitute;
using NSubstitute.Extensions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            using var mock = ConfigureServiceClient(plateFile, level, x, y, containerName, blobFormat)
                .Build();
            var pyramid = mock.Resolve<AzurePlateTilePyramid>();

            // Act
            using var result = pyramid.GetStream(plateFile, level, x, y);

            // Assert
            Assert.NotNull(result);
            Assert.Same(mock.Resolve<DownloadResult>(), result);

            mock.Resolve<BlobContainerClient>().DidNotReceive().CreateIfNotExists();
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
            var options = new AzurePlateTilePyramidOptions
            {
                CreateContainer = isCreateCalled
            };

            using var mock = ConfigureServiceClient(plateFile, level, x, y)
                .Provide(options)
                .Build();
            var pyramid = mock.Resolve<AzurePlateTilePyramid>();

            // Act
            using var stream1 = pyramid.GetStream(plateFile, level, x, y);
            using var stream2 = pyramid.GetStream(plateFile, level, x, y);

            // Assert
            Assert.Same(stream1, stream2);

            if (isCreateCalled)
            {
                mock.Resolve<BlobContainerClient>().Received(1).CreateIfNotExistsAsync();
            }
            else
            {
                mock.Resolve<BlobContainerClient>().DidNotReceive().CreateIfNotExistsAsync();
            }
        }

        [Theory]
        [InlineData("test.plate", 0, 0, 0, true, null, null)]
        [InlineData("test.plate", 0, 0, 0, false, null, null)]
        [InlineData("dssterrapixel.plate", 0, 0, 0, true, "dss", "DSSTerraPixelL{0}X{1}Y{2}.png")]
        [InlineData("dssterrapixel.plate", 0, 0, 0, false, "dss", "DSSTerraPixelL{0}X{1}Y{2}.png")]
        public async Task SaveStreamTests(string plateFile, int level, int x, int y, bool overwrite, string containerName, string blobFormat)
        {
            // Arrange
            using var container = ConfigureServiceClient(plateFile, level, x, y, containerName, blobFormat)
                .Provide(new AzurePlateTilePyramidOptions
                {
                    OverwriteExisting = overwrite
                })
                .Build();
            var stream = Substitute.ForPartsOf<Stream>();

            // Act
            await container.Resolve<AzurePlateTilePyramid>().SaveStreamAsync(stream, plateFile, level, x, y, default);

            // Assert
            await container.Resolve<BlobClient>().Received(1).UploadAsync(stream, overwrite, default);
        }

        private static AutoSubstituteBuilder ConfigureServiceClient(string plateFile, int level, int x, int y, string expectedContainerName = null, string blobFormat = null)
        {
            blobFormat ??= "L{0}X{1}Y{2}.png";
            var blobName = string.Format(blobFormat, level, x, y);
            var containerName = expectedContainerName ?? plateFile.Replace(".plate", string.Empty);

            return AutoSubstitute.Configure()
                .SubstituteFor2<DownloadResult>().Provide(out var result).Configured()
                .SubstituteFor2<BlobClient>().Provide(out var blob).Configure(b =>
                {
                    var response = Substitute.ForPartsOf<Response<BlobDownloadInfo>>();
                    response.Value.Returns(BlobsModelFactory.BlobDownloadInfo(content: result.Value));

                    b.Configure()
                        .Download().Returns(response);

                    b.WhenForAnyArgs(b => b.Upload(Arg.Any<Stream>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()))
                        .DoNotCallBase();
                })
                .SubstituteFor2<BlobContainerClient>().Provide(out var container).Configure(c =>
                {
                    c.Configure()
                        .GetBlobClient(blobName).Returns(blob.Value);

                    c.Configure()
                        .CreateIfNotExists().Returns(Substitute.For<Response<BlobContainerInfo>>());
                })
                .SubstituteFor2<BlobServiceClient>().Provide(out var service).Configure(service =>
                {
                    service.Configure()
                        .GetBlobContainerClient(containerName).Returns(container.Value);
                });
        }

        public abstract class DownloadResult : Stream { }
    }
}

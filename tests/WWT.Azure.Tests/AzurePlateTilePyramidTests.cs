using Autofac;
using AutofacContrib.NSubstitute;
using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NSubstitute;
using NSubstitute.Extensions;
using System.IO;
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
        public async Task GetStreamTests(string plateFile, int level, int x, int y, string containerName, string blobFormat)
        {
            // Arrange
            using var mock = ConfigureServiceClient(plateFile, level, x, y, containerName, blobFormat)
                .Build();
            var pyramid = mock.Resolve<AzurePlateTilePyramid>();

            // Act
            using var result = await pyramid.GetStreamAsync(plateFile, level, x, y, default);

            // Assert
            Assert.NotNull(result);
            Assert.Same(mock.Resolve<DownloadResult>(), result);

            mock.Resolve<BlobContainerClient>().DidNotReceive().CreateIfNotExists();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateContainer(bool isCreateCalled)
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
            using var stream1 = await pyramid.GetStreamAsync(plateFile, level, x, y, default);
            using var stream2 = await pyramid.GetStreamAsync(plateFile, level, x, y, default);

            // Assert
            Assert.Same(stream1, stream2);

            if (isCreateCalled)
            {
                await mock.Resolve<BlobContainerClient>().Received(1).CreateIfNotExistsAsync();
            }
            else
            {
                await mock.Resolve<BlobContainerClient>().DidNotReceive().CreateIfNotExistsAsync();
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

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task VeriyExists(bool skipIfExists, bool exists)
        {
            // Arrange
            var plateFile = _fixture.Create<string>();
            var level = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var options = new AzurePlateTilePyramidOptions
            {
                SkipIfExists = skipIfExists,
            };

            using var mock = ConfigureServiceClient(plateFile, level, x, y)
              .Provide(options)
              .Build();

            mock.Resolve<BlobClient>().Configure().ExistsAsync().Returns(Response.FromValue(exists, null));

            var stream = Substitute.For<Stream>();

            // Act
            var result = await mock.Resolve<AzurePlateTilePyramid>().SaveStreamAsync(stream, plateFile, level, x, y, default);

            // Assert
            Assert.Equal(!skipIfExists || !exists, result);
        }

        private static AutoSubstituteBuilder ConfigureServiceClient(string plateFile, int level, int x, int y, string expectedContainerName = null, string blobFormat = null)
        {
            // For all the non dss plate files
            var blobName = $"{plateFile.Replace(".plate", string.Empty)}/L{level}X{x}Y{y}.png";
            var containerName = AzurePlateTilePyramidOptions.DefaultContainer;

            if (plateFile == "dssterrapixel.plate")
            {
                blobName = $"DSSTerraPixelL{level}X{x}Y{y}.png";
                containerName = "dss";
            }

            return AutoSubstitute.Configure()
                .InjectProperties()
                .MakeUnregisteredTypesPerLifetime()
                .SubstituteFor<Response<BlobDownloadInfo>>()
                .SubstituteFor<DownloadResult>()
                .Provide(ctx => BlobsModelFactory.BlobDownloadInfo(content: ctx.Resolve<DownloadResult>()))
                .SubstituteFor<BlobClient>()
                    .ResolveReturnValue(c => c.Download())
                .SubstituteFor<BlobContainerClient>()
                    .ResolveReturnValue(t => t.GetBlobClient(blobName))
                    .ConfigureSubstitute(c =>
                    {
                        c.Configure()
                            .CreateIfNotExists().Returns(Substitute.For<Response<BlobContainerInfo>>());
                    })
                .SubstituteFor<BlobServiceClient>()
                    .ResolveReturnValue(service => service.GetBlobContainerClient(containerName));
        }
    }
}

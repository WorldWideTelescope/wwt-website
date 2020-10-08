using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using NSubstitute.Extensions;
using System.IO;
using System.Linq;
using System.Web;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class DSSTests
    {
        private readonly Fixture _fixture;

        public DSSTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(13)]
        public void TooLarge(int level)
        {
            // Arrange
            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .ConfigureParameterQ(level, 2, 3)
                .Build();

            // Act
            container.RunProviderTest<DSSProvider>();

            // Assert
            container.Resolve<IResponse>().Received(1).Write("No image");
        }

        [Theory]
        [InlineData(7)]
        [InlineData(0)]
        public void LowLevels(int level)
        {
            // Arrange
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            var data = _fixture.CreateMany<byte>(10);
            var result = new MemoryStream(data.ToArray());
            var outputStream = new MemoryStream();

            container.Resolve<IResponse>().Configure().OutputStream.Returns(outputStream);
            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, "dssterrapixel.plate", level, x, y).Returns(result);

            // Act
            container.RunProviderTest<DSSProvider>();

            // Assert
            Assert.Equal("image/png", container.Resolve<IResponse>().ContentType);
            Assert.Equal(data, outputStream.ToArray());
        }

        [Theory]
        [InlineData(8, 1, 2, 0, 0, 3, 1, 2)]
        [InlineData(9, 3, 3, 0, 0, 4, 3, 3)]
        [InlineData(10, 6, 3, 0, 0, 5, 6, 3)]
        [InlineData(11, 1, 3, 0, 0, 6, 1, 3)]
        public void HighLevels(int level, int x, int y, int fileX, int fileY, int level2, int x2, int y2)
        {
            // Arrange
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            var data = _fixture.CreateMany<byte>(10);
            var result = new MemoryStream(data.ToArray());
            var outputStream = new MemoryStream();
            var filename = $"DSSpngL5to12_x{fileX}_y{fileY}.plate";

            container.Resolve<IResponse>().Configure().OutputStream.Returns(outputStream);
            container.Resolve<IPlateTilePyramid>().GetStream(options.DssTerapixelDir, filename, level2, x2, y2).Returns(result);

            // Act
            container.RunProviderTest<DSSProvider>();

            // Assert
            Assert.Equal("image/png", container.Resolve<IResponse>().ContentType);
            Assert.Equal(data, outputStream.ToArray());
        }
    }
}

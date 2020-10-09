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
    public class g360ProviderTests
    {
        private readonly Fixture _fixture;

        public g360ProviderTests()
        {
            _fixture = new Fixture();
        }

        private static string GetPlateName(int x, int y, int level)
        {
            var index = DirectoryEntry.ComputeHash(level + 128, x, y) % 16;

            return $"g360-{index}.plate";
        }

        [InlineData(0, 0, 0)]
        [Theory]
        public void ExpectedTests(int level, int x, int y)
        {
            // Arrange
            var data = _fixture.CreateMany<byte>().ToArray();
            var options = _fixture.Create<FilePathOptions>();
            var plateName = GetPlateName(level, x, y);

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, plateName, -1, level, x, y).Returns(new MemoryStream(data));

            // Act
            container.RunProviderTest<g360Provider>();

            // Assert
            container.Resolve<IPlateTilePyramid>().Received(1).GetStream(options.WwtTilesDir, plateName, -1, level, x, y);
            Assert.Equal(data, container.GetOutputData());
        }

        [InlineData(false)]
        [InlineData(true)]
        [Theory]
        public void EmptyResult(bool isNull)
        {
            // Arrange
            var level = _fixture.Create<int>() % 12; // Level must be < 12;
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var options = _fixture.Create<FilePathOptions>();
            var plateName = GetPlateName(level, x, y);

            using var container = AutoSubstitute.Configure()
                .Provide(options)
                .InitializeProviderTests()
                .ConfigureParameterQ(level, x, y)
                .Build();

            if (isNull)
            {
                container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, plateName, -1, level, x, y).Returns((Stream)null);
            }
            else
            {
                var empty = Substitute.ForPartsOf<Stream>();
                empty.Configure().Length.Returns(0);
                container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, plateName, -1, level, x, y).Returns(empty);
            }

            // Act
            container.RunProviderTest<g360Provider>();

            // Assert
            container.Resolve<IResponse>().Received(1).Write("No image");
            Assert.Equal("text/plain", container.Resolve<IResponse>().ContentType);
        }

        [InlineData(13)]
        [Theory]
        public void LevelTooHigh(int level)
        {
            // Arrange
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .Provide(options)
                .InitializeProviderTests()
                .ConfigureParameterQ(level, x, y)
                .Build();

            // Act
            container.RunProviderTest<g360Provider>();

            // Assert
            container.Resolve<IResponse>().DidNotReceiveWithAnyArgs().Write(Arg.Any<string>());
            Assert.Empty(container.Resolve<IResponse>().ContentType);
        }
    }
}

using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Web;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class Galex4FarProviderTests
    {
        private const string PlateName = "Galex4Far_L0to8_x0_y0.plate";

        private readonly Fixture _fixture;

        public Galex4FarProviderTests()
        {
            _fixture = new Fixture();
        }

        [InlineData(0, 0, 0)]
        [InlineData(8, 0, 0)]
        [Theory]
        public void ExpectedTests(int level, int x, int y)
        {
            // Arrange
            var data = _fixture.CreateMany<byte>().ToArray();
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, PlateName, level, x, y).Returns(new MemoryStream(data));

            // Act
            container.RunProviderTest<Galex4FarProvider>();

            // Assert
            container.Resolve<IPlateTilePyramid>().Received(1).GetStream(options.WwtTilesDir, PlateName, level, x, y);
            Assert.Equal(data, container.GetOutputData());
        }


        [InlineData(9, 0, 0)]
        [Theory]
        public void ExpectedTestsLevel9(int level, int x, int y)
        {
            // Arrange
            var powLev3Diff = (int)Math.Pow(2, level - 3);
            var X8 = x / powLev3Diff;
            var Y8 = y / powLev3Diff;
            var L3 = level - 3;
            var X3 = x % powLev3Diff;
            var Y3 = y % powLev3Diff;
            var plateName = $"Galex4Far_L3to10_x{X8}_y{Y8}.plate";
            var data = _fixture.CreateMany<byte>().ToArray();
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, plateName, L3, X3, Y3).Returns(new MemoryStream(data));

            // Act
            container.RunProviderTest<Galex4FarProvider>();

            // Assert
            container.Resolve<IPlateTilePyramid>().Received(1).GetStream(options.WwtTilesDir, plateName, L3, X3, Y3);
            Assert.Equal(data, container.GetOutputData());
        }

        [InlineData(0, 0, 0, 0, PlateName)]
        [InlineData(8, 0, 0, 8, PlateName)]
        [InlineData(9, 0, 0, 6, "Galex4Far_L3to10_x0_y0.plate")]
        [Theory]
        public void ErrorInStream(int level, int x, int y, int passedLevel, string plateName)
        {
            // Arrange
            var data = _fixture.CreateMany<byte>().ToArray();
            var options = _fixture.Create<FilePathOptions>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests()
                .Provide(options)
                .ConfigureParameterQ(level, x, y)
                .Build();

            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, plateName, passedLevel, x, y).Returns(_ => throw new InvalidOperationException());

            // Act
            container.RunProviderTest<Galex4FarProvider>();

            // Assert
            container.Resolve<IPlateTilePyramid>().Received(1).GetStream(options.WwtTilesDir, plateName, passedLevel, x, y);
            container.Resolve<IResponse>().Received(1).Write("No image");
            Assert.Equal("text/plain", container.Resolve<IResponse>().ContentType);
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

            using var container = AutoSubstitute.Configure()
                .Provide(options)
                .InitializeProviderTests()
                .ConfigureParameterQ(level, x, y)
                .Build();

            if (isNull)
            {
                container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, PlateName, -1, level, x, y).Returns((Stream)null);
            }
            else
            {
                var empty = Substitute.ForPartsOf<Stream>();
                empty.Configure().Length.Returns(0);
                container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, PlateName, -1, level, x, y).Returns(empty);
            }

            // Act
            container.RunProviderTest<Galex4FarProvider>();

            // Assert
            container.Resolve<IResponse>().Received(1).Write("No image");
            Assert.Equal("text/plain", container.Resolve<IResponse>().ContentType);
        }

        [InlineData(11)]
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
            container.RunProviderTest<Galex4FarProvider>();

            // Assert
            container.Resolve<IResponse>().Received(1).Write("No image");
            Assert.Equal("text/plain", container.Resolve<IResponse>().ContentType);
        }
    }
}

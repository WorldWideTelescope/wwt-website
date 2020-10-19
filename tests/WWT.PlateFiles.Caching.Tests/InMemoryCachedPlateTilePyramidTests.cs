using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using System.IO;
using System.Linq;
using WWTWebservices;
using Xunit;

namespace WWT.PlateFiles.Caching.Tests
{
    public class InMemoryCachedPlateTilePyramidTests
    {
        private readonly Fixture _fixture;

        public InMemoryCachedPlateTilePyramidTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void CacheSanityPlate1()
        {
            // Arrange
            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Build();

            var level = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var prefix = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var expected = _fixture.CreateMany<byte>(10).ToArray();

            mock.Resolve<IPlateTilePyramid>().GetStream(prefix, name, level, x, y).Returns(new MemoryStream(expected));

            var cached = mock.Resolve<InMemoryCachedPlateTilePyramid>();

            // Act
            using var result1 = cached.GetStream(prefix, name, level, x, y);
            using var result2 = cached.GetStream(prefix, name, level, x, y);
            using var result3 = cached.GetStream(prefix, name, level, x, y);

            // Assert
            var ms1 = Assert.IsType<MemoryStream>(result1);
            var ms2 = Assert.IsType<MemoryStream>(result2);
            var ms3 = Assert.IsType<MemoryStream>(result3);

            Assert.Equal(expected, ms1.ToArray());
            Assert.Equal(expected, ms2.ToArray());
            Assert.Equal(expected, ms3.ToArray());

            mock.Resolve<IPlateTilePyramid>().Received(1).GetStream(prefix, name, level, x, y);
        }

        [Fact]
        public void CacheSanityPlate2()
        {
            // Arrange
            using var mock = AutoSubstitute.Configure()
                .MakeUnregisteredTypesPerLifetime()
                .Build();

            var level = _fixture.Create<int>();
            var tag = _fixture.Create<int>();
            var x = _fixture.Create<int>();
            var y = _fixture.Create<int>();
            var prefix = _fixture.Create<string>();
            var name = _fixture.Create<string>();
            var expected = _fixture.CreateMany<byte>(10).ToArray();

            mock.Resolve<IPlateTilePyramid>().GetStream(prefix, name, tag, level, x, y).Returns(new MemoryStream(expected));

            var cached = mock.Resolve<InMemoryCachedPlateTilePyramid>();

            // Act
            using var result1 = cached.GetStream(prefix, name, tag, level, x, y);
            using var result2 = cached.GetStream(prefix, name, tag, level, x, y);
            using var result3 = cached.GetStream(prefix, name, tag, level, x, y);

            // Assert
            var ms1 = Assert.IsType<MemoryStream>(result1);
            var ms2 = Assert.IsType<MemoryStream>(result2);
            var ms3 = Assert.IsType<MemoryStream>(result3);

            Assert.Equal(expected, ms1.ToArray());
            Assert.Equal(expected, ms2.ToArray());
            Assert.Equal(expected, ms3.ToArray());

            mock.Resolve<IPlateTilePyramid>().Received(1).GetStream(prefix, name, tag, level, x, y);
        }
    }
}

using AutofacContrib.NSubstitute;
using AutoFixture;
using System.IO;
using System.Linq;
using Xunit;

namespace WWT.PlateFiles.Tests
{
    public class StreamSliceTests
    {
        private readonly Fixture _fixture;

        public StreamSliceTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void SanityTest()
        {
            // Arrange
            var bytes = _fixture.CreateMany<byte>().ToArray();
            var offset = 1;
            var length = bytes.Length - 2;

            var subset = bytes.Skip(offset).Take(length);

            // Act
            using var ms = new MemoryStream(bytes);
            using var sub = new StreamSlice(ms, offset, length);

            // Assert
            Assert.Equal(length, sub.Length);
            Assert.Equal(offset, ms.Position);
            Assert.Equal(sub.ToArray(), subset);
        }

        [InlineData(500, 300)]
        [Theory]
        public void ReallyLongStream(int offset, int length)
        {
            // Arrange
            var bytes = new byte[1024 * 1024];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)i;
            }

            var subset = bytes.Skip(offset).Take(length);

            // Act
            using var ms = new MemoryStream(bytes);
            using var sub = new StreamSlice(ms, offset, length);

            // Assert
            Assert.Equal(length, sub.Length);
            Assert.Equal(offset, ms.Position);
            Assert.Equal(0, sub.Position);
            Assert.Equal(sub.ToArray(), subset);
        }

        [InlineData(500, 300)]
        [Theory]
        public void ReallyLongStreamWithInitialSeek(int offset, int length)
        {
            // Arrange
            var bytes = new byte[1024 * 1024];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)i;
            }

            var subset = bytes.Skip(offset).Take(length);
            using var ms = new MemoryStream(bytes);

            ms.Seek(30, SeekOrigin.Begin);

            // Act
            using var sub = new StreamSlice(ms, offset, length);

            // Assert
            Assert.Equal(length, sub.Length);
            Assert.Equal(offset, ms.Position);
            Assert.Equal(0, sub.Position);
            Assert.Equal(sub.ToArray(), subset);
        }
    }
}

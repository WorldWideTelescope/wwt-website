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
    public class DustToastProviderTests
    {
        private readonly Fixture _fixture;

        public DustToastProviderTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(8)]
        public void TooLarge(int level)
        {
            // Arrange
            using var container = AutoSubstitute.Configure()
                .InitializeHttpWrappers()
                .ConfigureParameters(a => a.Add("Q", $"{level},2,3"))
                .Build();

            // Act
            container.RunProviderTest<DustToastProvider>();

            // Assert
            Assert.Empty(container.Resolve<HttpResponseBase>().ContentType);
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
                .InitializeHttpWrappers()
                .Provide(options)
                .ConfigureParameters(a => a.Add("Q", $"{level},{x},{y}"))
                .Build();

            var data = _fixture.CreateMany<byte>(10);

            container.Resolve<IPlateTilePyramid>().GetStream(options.WwtTilesDir, "dust.plate", level, x, y).Returns(new MemoryStream(data.ToArray()));

            // Act
            container.RunProviderTest<DustToastProvider>();

            // Assert
            Assert.Equal("image/png", container.Resolve<HttpResponseBase>().ContentType);
            Assert.Equal(data, container.GetOutputData());
        }
    }
}

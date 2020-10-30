using AutofacContrib.NSubstitute;
using AutoFixture;
using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class Tiles2ProviderTests : ProviderTests<Tiles2Provider>
    {
        private readonly string _fileName;

        public Tiles2ProviderTests()
        {
            _fileName = Fixture.Create<string>();
        }

        [Fact]
        public async Task NotKnownPlateFile()
        {
            // Arrange
            var x = Fixture.Create<int>();
            var y = Fixture.Create<int>();

            using var container = AutoSubstitute.Configure()
                .InitializeProviderTests(initializeKnownPlateFile: false)
                .Provide(Options)
                .ConfigureParameterQ(0, x, y, _fileName)
                .Build();

            // Act
            await container.RunProviderTestAsync<Tiles2Provider>();

            // Assert
            GetStreamFromPlateTilePyramid(container.Resolve<IPlateTilePyramid>().DidNotReceive(), 0, x, y);
            ExpectedResponseAboveMaxLevel(container.Resolve<IResponse>());
        }

        protected override object[] GetParameterQ(int level, int x, int y)
            => new object[] { level, x, y, _fileName };

        protected override int MaxLevel => 9;

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStream(Options.WwtTilesDir, $"{_fileName}.plate", -1, level, x, y);

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.ContentType);
            Assert.Empty(response.OutputStream.ToArray());
        }
    }
}

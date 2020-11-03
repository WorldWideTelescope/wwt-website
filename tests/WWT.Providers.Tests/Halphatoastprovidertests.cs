using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class hAlphaToastProviderTests : ProviderTests<HAlphaToastProvider>
    {
        protected override int MaxLevel => 8;

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStreamAsync(Options.WwtTilesDir, "halpha.plate", level, x, y, default);
    }
}

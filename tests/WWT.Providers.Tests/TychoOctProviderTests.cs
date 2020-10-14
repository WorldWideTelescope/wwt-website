using System;
using System.IO;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class TychoOctProviderTests : ProviderTests<TychoOctProvider>
    {
        protected override int MaxLevel => 7;

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.ContentType);
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStream(Options.WwtTilesDir, $"tychotoast.plate", level, x, y);
    }
}

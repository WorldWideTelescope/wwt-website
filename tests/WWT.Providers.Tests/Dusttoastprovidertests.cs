using System;
using System.IO;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class DustToastProviderTests : ProviderTests<DustToastProvider>
    {
        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override int MaxLevel => 7;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            return plateTiles.GetStream(Options.WwtTilesDir, "dust.plate", level, x, y);
        }
    }
}


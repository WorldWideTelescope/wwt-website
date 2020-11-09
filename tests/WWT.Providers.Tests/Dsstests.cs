using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class DSSTests : ProviderTests<DSSProvider>
    {
        protected override int MaxLevel => 12;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level > 12)
            {
                return null;
            }
            else if (level < 8)
            {
                return plateTiles.GetStreamAsync(Options.WwtTilesDir, "DSSTerraPixel.plate", level, x, y, default);
            }
            else
            {
                var powLev5Diff = (int)Math.Pow(2, level - 5);
                var X32 = x / powLev5Diff;
                var Y32 = y / powLev5Diff;

                var L5 = level - 5;
                var X5 = x % powLev5Diff;
                var Y5 = y % powLev5Diff;

                return plateTiles.GetStreamAsync(Options.DssTerapixelDir, $"DSSpngL5to12_x{X32}_y{Y32}.plate", L5, X5, Y5, default);
            }
        }

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            response.Received(1).WriteAsync("No image", default);
        }
    }
}

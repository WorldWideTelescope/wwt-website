using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class DSSToastTests : ProviderTests<DSSToastProvider>
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
                return plateTiles.GetStreamAsync(Options.WwtTilesDir, "DSSToast.plate", level, x, y, default);
            }
            else
            {
                var powLev5Diff = (int)Math.Pow(2, level - 5);

                var L5 = level - 5;
                var X5 = x % powLev5Diff;
                var Y5 = y % powLev5Diff;

                var filename = $"DSSpngL5to12_x{x / powLev5Diff}_y{y / powLev5Diff}.plate";

                return plateTiles.GetStreamAsync(Options.DssToastPng, filename, L5, X5, Y5, default);
            }
        }

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            response.Received(1).WriteAsync("No image", default);
        }
    }
}

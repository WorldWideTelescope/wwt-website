using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

using WWT.PlateFiles;

namespace WWT.Providers.Tests
{
    public class MoontoastdemProviderTests : ProviderTests<MoontoastdemProvider>
    {
        protected override int MaxLevel => 10;

        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.ContentType);
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level < 7)
            {
                return plateTiles.GetStreamAsync(Path.Combine(Options.WWTDEMDir, "toast", "lola"), "moontoast_L0X0Y0.plate", level, x, y, default);
            }
            else
            {
                int powLev5Diff = (int)Math.Pow(2, level - 3);
                int X32 = x / powLev5Diff;
                int Y32 = y / powLev5Diff;

                int L5 = level - 3;
                int X5 = x % powLev5Diff;
                int Y5 = y % powLev5Diff;

                return plateTiles.GetStreamAsync(Path.Combine(Options.WWTDEMDir, "toast", "lola"), $"moontoast_L3X{X32}Y{Y32}.plate", L5, X5, Y5, default);
            }
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class GlimpseProviderTests : ProviderTests<GlimpseProvider>
    {
        protected override int MaxLevel => 11;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level == 11)
            {
                int powLev3Diff = (int)Math.Pow(2, level - 1);
                int X8 = x / powLev3Diff;
                int Y8 = y / powLev3Diff;

                int L3 = level - 1;
                int X3 = x % powLev3Diff;
                int Y3 = y % powLev3Diff;

                return plateTiles.GetStreamAsync(Options.WwtTilesDir, $"glimpse_L1to11_x{X8}_y{Y8}.plate", L3, X3, Y3, default);
            }
            else
            {
                return plateTiles.GetStreamAsync(Options.WwtTilesDir, "glimpse_L0to10_x0_y0.plate", level, x, y, default);
            }
        }
    }
}

using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class MipsgalProviderTests : ProviderTests<MipsgalProvider>
    {
        protected override int MaxLevel => 11;

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level < 11)
            {
                return plateTiles.GetStream(Options.WwtTilesDir, "mipsgal_L0to10_x0_y0.plate", level, x, y);
            }
            else
            {
                int powLev3Diff = (int)Math.Pow(2, level - 1);
                int X8 = x / powLev3Diff;
                int Y8 = y / powLev3Diff;

                int L3 = level - 1;
                int X3 = x % powLev3Diff;
                int Y3 = y % powLev3Diff;

                return plateTiles.GetStream(Options.WwtTilesDir, $"mipsgal_L1to11_x{X8}_y{Y8}.plate", L3, X3, Y3);
            }
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers.Tests
{
    public class GalexToastProviderTests : ProviderTests<GalexToastProvider>
    {
        protected override int MaxLevel => 10;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level == 9 || level == 10)
            {
                int powLev3Diff = (int)Math.Pow(2, level - 3);
                int X8 = x / powLev3Diff;
                int Y8 = y / powLev3Diff;

                int L3 = level - 3;
                int X3 = x % powLev3Diff;
                int Y3 = y % powLev3Diff;

                var plateName = $"GalexBoth_L3to10_x{X8}_y{Y8}.plate";

                return plateTiles.GetStreamAsync(Options.WwtGalexDir, plateName, L3, X3, Y3, default);
            }
            else
            {
                return plateTiles.GetStreamAsync(Options.WwtTilesDir, "GalexBoth_L0to8_x0_y0.plate", level, x, y, default);
            }
        }
    }
}

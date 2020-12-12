using System;
using System.IO;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers.Tests
{
    public class Galex4FarProviderTests : ProviderTests<Galex4FarProvider>
    {
        protected override int MaxLevel => 10;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            if (level == 9 || level == 10)
            {
                var powLev3Diff = (int)Math.Pow(2, level - 3);
                var X8 = x / powLev3Diff;
                var Y8 = y / powLev3Diff;

                var L3 = level - 3;
                var X3 = x % powLev3Diff;
                var Y3 = y % powLev3Diff;

                var plateName = $"Galex4Far_L3to10_x{X8}_y{Y8}.plate";

                return plateTiles.GetStreamAsync(Options.WwtTilesDir, plateName, L3, X3, Y3, default);
            }

            return plateTiles.GetStreamAsync(Options.WwtTilesDir, "Galex4Far_L0to8_x0_y0.plate", level, x, y, default);
        }
    }
}

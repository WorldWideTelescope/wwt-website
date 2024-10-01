using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

namespace WWT.Providers;

public class DSSProvider(IPlateTilePyramid plateTile, WwtOptions options)
{
    public async Task<Stream> GetStreamAsync(int level, int tileX, int tileY, CancellationToken token)
    {
        if (level > 12)
        {
            return null;
        }
        else if (level < 8)
        {
            return await plateTile.GetStreamAsync(options.WwtTilesDir, "DSSTerraPixel.plate", level, tileX, tileY, token);
        }
        else
        {
            int powLev5Diff = (int)Math.Pow(2, level - 5);
            int X32 = tileX / powLev5Diff;
            int Y32 = tileY / powLev5Diff;

            int L5 = level - 5;
            int X5 = tileX % powLev5Diff;
            int Y5 = tileY % powLev5Diff;

            string filename = $"DSSPngL5to12_x{X32}_y{Y32}.plate";

            return await plateTile.GetStreamAsync(options.DssTerapixelDir, filename, L5, X5, Y5, token);
        }
    }
}

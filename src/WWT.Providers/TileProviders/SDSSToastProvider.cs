#nullable enable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers;

public class SDSSToastProvider(IPlateTilePyramid plateTiles, WwtOptions options, IOctTileMapBuilder octTileMap)
{
    public async Task<Stream?> GetStreamAsync(int level, int tileX, int tileY, CancellationToken token)
    {
        string wwtTilesDir = options.WwtTilesDir;

        if (level > 14)
        {
            return null;
        }

        if (level < 9)
        {
            var s = await plateTiles.GetStreamAsync(wwtTilesDir, "SDSS_8.plate", level, tileX, tileY, token);

            if (s.Length == 0)
            {
                return null;
            }

            return s;
        }

        return await octTileMap.GetOctTileAsync(level, tileX, tileY, enforceBoundary: true, token: token);
    }
}

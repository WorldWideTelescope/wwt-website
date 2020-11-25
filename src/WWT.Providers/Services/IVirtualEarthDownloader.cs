#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public interface IVirtualEarthDownloader
    {
        int GetServerID(int x, int y);

        Task<Stream> DownloadVeTileAsync(VirtualEarthTile tileType, int level, int tileX, int tileY, CancellationToken token);

        int GetTileAddressFromVEKey(string veKey, out int x, out int y);

        string GetTileID(int x, int y, int level, bool GoogleStyle);
    }
}

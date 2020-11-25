#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    public interface ITileCreator
    {
        Task<bool> ExistsAsync(CancellationToken token);

        Task AddTileAsync(Stream tile, int level, int x, int y, CancellationToken token);

        Task AddThumbnailAsync(Stream thumbnail, CancellationToken token);
    }
}

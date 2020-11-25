#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    public interface ITileAccessor
    {
        Task<Stream> GetTileAsync(string id, int level, int x, int y, CancellationToken token);

        Task<Stream> GetThumbnailAsync(string name, CancellationToken token);

        ITileCreator CreateTile(string id);
    }
}

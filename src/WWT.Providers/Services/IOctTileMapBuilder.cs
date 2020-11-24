#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public interface IOctTileMapBuilder
    {
        Task<Stream> GetOctTileAsync(int level, int tileX, int tileY, bool enforceBoundary = false, CancellationToken token = default);
    }
}

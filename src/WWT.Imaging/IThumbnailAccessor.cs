#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    public interface IThumbnailAccessor
    {
        Task<Stream> GetThumbnailStreamAsync(string name, string type, CancellationToken token);

        Task<Stream> GetDefaultThumbnailStreamAsync(CancellationToken token);
    }
}

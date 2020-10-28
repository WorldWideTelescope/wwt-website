using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Tours
{
    public interface ITourAccessor
    {
        Task<Stream> GetTourAsync(string id, CancellationToken token);

        Task<Stream> GetTourThumbnailAsync(string id, CancellationToken token);

        Task<Stream> GetAuthorThumbnailAsync(string id, CancellationToken token);
    }
}

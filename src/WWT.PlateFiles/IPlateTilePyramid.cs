using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWTWebservices
{
    public interface IPlateTilePyramid
    {
        Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int level, int x, int y, CancellationToken token);

        Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int tag, int level, int x, int y, CancellationToken token);

        IAsyncEnumerable<string> GetPlateNames(CancellationToken token);
    }
}

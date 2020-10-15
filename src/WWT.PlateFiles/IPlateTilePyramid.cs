using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WWTWebservices
{
    public interface IPlateTilePyramid
    {
        Stream GetStream(string pathPrefix, string plateName, int level, int x, int y);

        Stream GetStream(string pathPrefix, string plateName, int tag, int level, int x, int y);

        IAsyncEnumerable<string> GetPlateNames(CancellationToken token);
    }
}

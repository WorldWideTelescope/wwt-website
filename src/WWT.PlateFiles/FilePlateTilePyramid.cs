using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WWTWebservices
{
    public class FilePlateTilePyramid : IPlateTilePyramid
    {
        public async IAsyncEnumerable<string> GetPlateNames([EnumeratorCancellation]CancellationToken token)
        {
            await Task.Yield();
            yield break;
        }

        public Stream GetStream(string pathPrefix, string plateName, int level, int x, int y)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                throw new System.ArgumentException($"'{nameof(pathPrefix)}' cannot be null or empty", nameof(pathPrefix));
            }

            return PlateTilePyramid.GetFileStream(Path.Combine(pathPrefix, plateName), level, x, y);
        }

        public Stream GetStream(string pathPrefix, string plateName, int tag, int level, int x, int y)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                throw new System.ArgumentException($"'{nameof(pathPrefix)}' cannot be null or empty", nameof(pathPrefix));
            }
            var plateFile2 = new PlateFile2(Path.Combine(pathPrefix, plateName));
            return plateFile2.GetFileStream(tag, level, x, y);
        }
    }
}

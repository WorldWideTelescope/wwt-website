using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT
{
    public class FilePlateTilePyramid : IPlateTilePyramid
    {
        private readonly string _directory;

        public FilePlateTilePyramid(string directory)
        {
            _directory = directory;
        }

        public async IAsyncEnumerable<string> GetPlateNames([EnumeratorCancellation] CancellationToken token)
        {
            await Task.Yield();

            foreach (var file in Directory.GetFiles(_directory, "*.plate"))
            {
                yield return Path.GetFileName(file);
            }
        }

        public Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int level, int x, int y, CancellationToken token)
        {
            var result = PlateTilePyramid.GetFileStream(Path.Combine(_directory, plateName), level, x, y);

            return Task.FromResult(result);
        }

        public Task<Stream> GetStreamAsync(string pathPrefix, string plateName, int tag, int level, int x, int y, CancellationToken token)
        {
            var plateFile2 = new PlateFile2(Path.Combine(_directory, plateName));
            var result = plateFile2.GetFileStream(tag, level, x, y);

            return Task.FromResult(result);
        }
    }
}

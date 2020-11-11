using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

namespace WWT.Providers
{
    public class DevDataAccessor : IDevDataAccessor
    {
        private readonly IPlateTileDownloader _downloader;

        private readonly string[] _plateFiles = new[]
        {
            "DSSTerraPixel.plate"
        };

        public DevDataAccessor(IPlateTileDownloader downloader)
        {
            _downloader = downloader;
        }

        public async Task<Stream> GetDevDataAsync(int maxLevel, CancellationToken token)
        {
            var ms = new MemoryStream();

            using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, leaveOpen: true))
            {
                foreach (var plateFile in _plateFiles)
                {
                    var entry = zip.CreateEntry($"coredata/{plateFile}");
                    using var stream = entry.Open();

                    await _downloader.DownloadPlateFileAsync(plateFile, stream, maxLevel, token);
                }
            }

            ms.Position = 0;
            return ms;
        }
    }
}

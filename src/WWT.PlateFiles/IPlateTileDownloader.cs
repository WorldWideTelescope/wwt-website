using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.PlateFiles
{
    public interface IPlateTileDownloader
    {
        IAsyncEnumerable<string> GetPlateNames(CancellationToken token);

        Task<int> DownloadPlateFileAsync(string name, Stream stream, int maxLevel, CancellationToken token);
    }
}

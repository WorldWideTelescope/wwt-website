using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Imaging
{
    public interface IToastTileMapBuilder
    {
        Task<Stream> CreateToastTileMapAsync(string wmsUrl, int level, int tileX, int tileY, ImageSource imageSource, CancellationToken token);

        string GetToastTileMapAddress(string wmsUrl, int level, int tileX, int tileY, ImageSource imageSource);
    }
}

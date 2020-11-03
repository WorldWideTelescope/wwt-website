using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Imaging
{
    public class ToastTileMapBuilder : IToastTileMapBuilder
    {
        private const int sqSide = 256;

        private readonly HttpClient _httpClient;

        public ToastTileMapBuilder()
        {
            _httpClient = new HttpClient();
        }

        public string GetToastTileMapAddress(string wmsUrl, int level, int tileX, int tileY, ImageSource imageSource)
        {
            var map = new ToastTileMap(level, tileX, tileY);

            WMSImage sdim = new WMSImage(map.raMin, map.decMax, map.raMax, map.decMin);

            return sdim.GetImageUrl(wmsUrl, imageSource);
        }

        public async Task<Stream> CreateToastTileMapAsync(string wmsUrl, int level, int tileX, int tileY, ImageSource imageSource, CancellationToken token)
        {
            ToastTileMap map = new ToastTileMap(level, tileX, tileY);

            using Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
            using FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
            using WMSImage sdim = new WMSImage(map.raMin, map.decMax, map.raMax, map.decMin);

            var address = sdim.GetImageUrl(wmsUrl, imageSource);

            using var result = await _httpClient.GetAsync(address, token);
            using var stream = await result.Content.ReadAsStreamAsync();

            sdim.Image = new Bitmap(stream);
            sdim.Lock();

            bmpOutputFast.LockBitmap();

            // Fill up bmp from sdim
            Vector2d vxy, vradec;
            unsafe
            {
                for (int y = 0; y < sqSide; y++)
                {
                    var pPixel = bmpOutputFast[0, y];
                    vxy.Y = (y / 255.0);
                    for (int x = 0; x < sqSide; x++)
                    {
                        vxy.X = (x / 255.0);
                        vradec = map.PointToRaDec(vxy.X, vxy.Y);
                        *pPixel = sdim.GetPixelDataAtRaDec(vradec);
                        pPixel++;
                    }
                }
            }

            var ms = new MemoryStream();
            bmpOutputFast.UnlockBitmap();

            bmpOutput.Save(ms, ImageFormat.Png);

            ms.Position = 0;

            return ms;
        }
    }
}

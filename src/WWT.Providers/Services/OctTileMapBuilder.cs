
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.Imaging;

namespace WWT.Providers
{
    public class OctTileMapBuilder : IOctTileMapBuilder
    {
        public Task<Stream?> GetOctTileAsync(int level, int tileX, int tileY, bool enforceBoundary, CancellationToken token)
        {
            var map = new OctTileMap(level, tileX, tileY);

            // SDSS boundaries
            // RA: 105 deg <-> 270 deg
            // DEC: -3 deg <-> + 75 deg
            if (enforceBoundary)
            {
                if (map.raMin > 270 | map.decMax < -3 | map.raMax < 105 | map.decMin > 75)
                {
                    return Task.FromResult<Stream?>(null);
                }
            }

            Int32 sqSide = 256;

            using Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
            FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
            SdssImage sdim = new SdssImage(map.raMin, map.decMax, map.raMax, map.decMin, true);
            sdim.LoadImage();

            if (sdim.image != null)
            {

                sdim.Lock();

                bmpOutputFast.LockBitmap();
                // Fill up bmp from sdim

                Vector2d vxy, vradec;
                unsafe
                {
                    PixelData* pPixel;
                    for (int y = 0; y < sqSide; y++)
                    {
                        pPixel = bmpOutputFast[0, y];
                        vxy.Y = (y / 255.0);
                        for (int x = 0; x < sqSide; x++)
                        {
                            vxy.X = (x / 255.0);
                            vradec = map.PointToRaDec(vxy);
                            *pPixel = sdim.GetPixelDataAtRaDec(vradec);

                            pPixel++;
                        }
                    }
                }

                sdim.Unlock();
                sdim.image.Dispose();

                bmpOutputFast.UnlockBitmap();
            }

            var result = bmpOutput.SaveToStream(ImageFormat.Png);

            return Task.FromResult<Stream?>(result);
        }
    }
}

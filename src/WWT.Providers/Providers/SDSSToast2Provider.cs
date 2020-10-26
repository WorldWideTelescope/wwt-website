using OctSetTest;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class SDSSToast2Provider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public SDSSToast2Provider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 14)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return Task.CompletedTask;
            }

            if (level < 9)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = _plateTiles.GetStream(_options.WwtTilesDir, "sdss_8.plate", level, tileX, tileY))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return Task.CompletedTask;
                }
            }


            string filename = $@"{_options.DSSTileCache}\SDSSToast\{level}\{tileY}\{tileY}_{tileX}.png";
            /*   
                   if (!sdssTile )
                   {
                       // todo return black tile
                       using (Bitmap bmp = Bitmap(256, 256))
                       {
                           using (Graphics g = Graphics.FromImage(bmp))
                           {
                               g.Clear(Color.Black);
                           }
                           bmp.Save(filename);
                           context.Response.WriteFile(filename);
                       }
                       return;
                   }
                 */

            if (File.Exists(filename))
            {
                try
                {
                    context.Response.WriteFile(filename);
                    return Task.CompletedTask;
                }
                catch
                {
                }
            }
            else
            {
                OctTileMap map = new OctTileMap(level, tileX, tileY);

                Int32 sqSide = 256;

                //Vector2d topLeft = map.PointToRaDec(new Vector2d(0, 0))
                //map.PointToRaDec(new Vector2d(0, 1))
                //map.PointToRaDec(new Vector2d(1, 0))
                //map.PointToRaDec(new Vector2d(1, 1))


                // SDSS boundaries
                // RA: 105 deg <-> 270 deg
                // DEC: -3 deg <-> + 75 deg

                if (!(map.raMin > 270 | map.decMax < -3 | map.raMax < 105 | map.decMin > 75))
                {
                    Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
                    FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
                    SdssImage sdim = new SdssImage(map.raMin, map.decMax, map.raMax, map.decMin);
                    sdim.LoadImage();
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


                    bmpOutputFast.UnlockBitmap();

                    string path = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    bmpOutput.Save(filename, ImageFormat.Png);
                    bmpOutput.Dispose();
                    try
                    {
                        context.Response.WriteFile(filename);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    //context.Response.WriteFile(@"c:\inetpub\cache\empty.png");
                    context.Response.Write("No Image");
                }
            }

            context.Response.End();

            return Task.CompletedTask;
        }
    }
}

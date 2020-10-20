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
    public class SDSSToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public SDSSToastProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (context.Request.UserAgent.ToLower().Contains("wget"))
            {
                context.Response.Write("You are not allowed to bulk download imagery thru the tile service. Please contact wwtpage@microsoft.com for more information.");
                context.Response.End();
                return Task.CompletedTask;
            }

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            //++
            // 2014-09-26 security fix.
            //
            int level = 0;
            int tileX = 0;
            int tileY = 0;
            try
            {
                level = Convert.ToInt32(values[0]);
                tileX = Convert.ToInt32(values[1]);
                tileY = Convert.ToInt32(values[2]);
            }
            catch
            {
                context.Response.Write("Invalid query string.");
                context.Response.End();
                return Task.CompletedTask;
            }

            string wwtTilesDir = _options.WwtTilesDir;

            if (level > 14)
            {
                context.Response.Write("No image");
                context.Response.End();
                return Task.CompletedTask;
            }

            if (level < 9)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = _plateTiles.GetStream(wwtTilesDir, "sdss_8.plate", level, tileX, tileY))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return Task.CompletedTask;
                    }

                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return Task.CompletedTask;
                }
            }

            string filename = $@"{_options.DSSTileCache}\SDSSToast\{level}\{tileY}\{tileY}_{tileX}.png";
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

                    sdim.Unlock();
                    sdim.image.Dispose();

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
                    context.Response.Write("No Image");
                }
            }

            context.Response.End();
            return Task.CompletedTask;
        }
    }
}

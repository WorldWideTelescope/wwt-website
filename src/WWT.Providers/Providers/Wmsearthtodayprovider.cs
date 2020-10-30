using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class WMSEarthTodayProvider : RequestProvider
    {
        private readonly IFileNameHasher _hasher;
        private readonly FilePathOptions _options;

        public WMSEarthTodayProvider(IFileNameHasher hasher, FilePathOptions options)
        {
            _hasher = hasher;
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            bool debug = context.Request.Params["debug"] != null;
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string wmsUrl = values[3];
            string dirPart = _hasher.HashName(wmsUrl).ToString();

            string DSSTileCache = _options.DSSTileCache;

            var filename = String.Format(DSSTileCache + "\\WMS\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, dirPart);
            var path = String.Format(DSSTileCache + "\\WMS\\{3}\\{0}\\{2}", level, tileX, tileY, dirPart);

            if (level > 15)
            {
                context.Response.Write("No image");
                context.Response.End();
                return Task.CompletedTask;
            }

            if (File.Exists(filename) && DateTime.Now.Subtract(File.GetLastWriteTime(filename)).TotalHours < 12)
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
                ToastTileMap map = new ToastTileMap(level, tileX, tileY);
                Int32 sqSide = 256;
                Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
                FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
                WMSImage sdim = new WMSImage(map.raMin, map.decMax, map.raMax, map.decMin);
                if (debug)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(sdim.LoadImage(wmsUrl, true, ImageSource.WmsJpl));
                    context.Response.End();
                    return Task.CompletedTask;
                }
                sdim.LoadImage(wmsUrl, false, ImageSource.WmsJpl);
                sdim.Lock();

                bmpOutputFast.LockBitmap();

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
                            vradec = map.PointToRaDec(vxy.X, vxy.Y);
                            *pPixel = sdim.GetPixelDataAtRaDec(vradec);


                            pPixel++;
                        }
                    }
                }
                //sdim.Unlock();
                bmpOutputFast.UnlockBitmap();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(filename))
                {
                    File.Delete(filename);
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

            context.Response.End();
            return Task.CompletedTask;
        }
    }
}

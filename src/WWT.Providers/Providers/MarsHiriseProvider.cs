using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class MarsHiriseProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public MarsHiriseProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            var id = values.Length > 3 ? Convert.ToInt32(values[3]) : -1;

            if (level > 17)
            {
                context.Response.StatusCode = 404;
                return;
            }

            using (Bitmap output = new Bitmap(256, 256))
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    int ll = level;
                    int xx = tileX;
                    int yy = tileY;

                    if (ll > 8)
                    {
                        int levelDif = ll - 8;
                        int scale = (int)Math.Pow(2, levelDif);
                        int tx = xx / scale;
                        int ty = yy / scale;

                        int offsetX = (xx - (tx * scale)) * (256 / scale);
                        int offsetY = (yy - (ty * scale)) * (256 / scale);
                        float width = Math.Max(2, (256 / scale));
                        float height = width;
                        if ((width + offsetX) >= 255)
                        {
                            width -= 1;
                        }
                        if ((height + offsetY) >= 255)
                        {
                            height -= 1;
                        }

                        using (var stream = _plateTiles.GetStream(_options.WwtTilesDir, "marsbasemap.plate", -1, 8, tx, ty))
                        using (var bmp1 = new Bitmap(stream))
                        {
                            g.DrawImage(bmp1, new RectangleF(0, 0, 256, 256), new RectangleF(offsetX, offsetY, width, height), GraphicsUnit.Pixel);
                        }
                    }
                    else
                    {
                        using (var stream = _plateTiles.GetStream(_options.WwtTilesDir, "marsbasemap.plate", -1, ll, xx, yy))
                        using (var bmp1 = new Bitmap(stream))
                        {
                            g.DrawImageUnscaled(bmp1, new Point(0, 0));
                        }
                    }

                    try
                    {
                        using (var stream = LoadHiRise(ll, xx, yy, id))
                        {
                            if (stream != null)
                            {
                                using (var bmp2 = new Bitmap(stream))
                                {
                                    g.DrawImageUnscaled(bmp2, new Point(0, 0));
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                output.Save(context.Response.OutputStream, ImageFormat.Png);
            }
        }

        private Stream LoadHiRise(int level, int tileX, int tileY, int id)
        {
            UInt32 index = ComputeHash(level, tileX, tileY) % 300;
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(String.Format(@"https://marsstage.blob.core.windows.net/hirise/hiriseV5_{0}.plate", index)));

            Stream stream = blob.OpenRead();

            return PlateFile2.GetFileStream(stream, id, level, tileX, tileY);
        }

        private UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}

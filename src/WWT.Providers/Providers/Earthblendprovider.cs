using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/EarthBlend.aspx")]
    public class EarthBlendProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;
        private readonly IVirtualEarthDownloader _veDownloader;

        public EarthBlendProvider(IPlateTilePyramid plateTiles, WwtOptions options, IVirtualEarthDownloader veDownloader)
        {
            _plateTiles = plateTiles;
            _options = options;
            _veDownloader = veDownloader;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string wwtTilesDir = _options.WwtTilesDir;
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 20)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
                return;
            }

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, "BmngMerBase.plate", level, tileX, tileY, token))
                {
                    int length = (int)s.Length;
                    byte[] data = new byte[length];
                    s.Read(data, 0, length);
                    await context.Response.OutputStream.WriteAsync(data, 0, length, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
            else if (level == 8)
            {
                int L = level;
                int X = tileX;
                int Y = tileY;
                int powLev5Diff = (int)Math.Pow(2, L - 2);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;

                int L5 = L - 2;
                int X5 = X % powLev5Diff;
                int Y5 = Y % powLev5Diff;

                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, $"BmngMerL2X{X32}Y{Y32}.plate", L5, X5, Y5, token))
                {
                    int length = (int)s.Length;
                    byte[] data = new byte[length];
                    s.Read(data, 0, length);
                    await context.Response.OutputStream.WriteAsync(data, 0, length, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }

            }
            else if (level == 9)
            {
                int L = level;
                int X = tileX;
                int Y = tileY;
                int powLev5Diff = (int)Math.Pow(2, L - 2);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;

                int L5 = L - 2;
                int X5 = X % powLev5Diff;
                int Y5 = Y % powLev5Diff;

                float[][] ptsArray =
                {
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, 0.5f, 0},
                        new float[] {0, 0, 0, 0, 1}
                    };

                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                ImageAttributes imgAttributes = new ImageAttributes();
                imgAttributes.SetColorMatrix(clrMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);
                context.Response.ContentType = "image/png";
                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, $"BmngMerL2X{X32}Y{Y32}.plate", L5, X5, Y5, token))
                {
                    using var bmp = new Bitmap(s);

                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        using var veTile = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level, tileX, tileY, token);
                        if (veTile.Length != 0 && veTile.Length != 1033)
                        {
                            using var temp = new Bitmap(veTile);

                            g.DrawImage(temp, new Rectangle(0, 0, 256, 256), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
                        }
                    }

                    bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                }
            }
            else
            {
                using var veTile = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level, tileX, tileY, token);
                await veTile.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}

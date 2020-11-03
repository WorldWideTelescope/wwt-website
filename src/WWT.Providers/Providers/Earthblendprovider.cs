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
    public class EarthBlendProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public EarthBlendProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string wwtTilesDir = _options.WwtTilesDir;
            string DSSTileCache = _options.DSSTileCache;
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 20)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return;
            }

            string filename = $@"{DSSTileCache}\EarthBlend\level{level}\{tileY}\{tileX}_{tileY}.jpg";
            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, "BmngMerBase.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
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
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }

            }
            else if (level == 9)
            {
                if (!File.Exists(filename))
                {
                    string path = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
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
                        Bitmap bmp = new Bitmap(s);
                        Graphics g = Graphics.FromImage(bmp);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        string tempName = WWTUtil.DownloadVeTile(level, tileX, tileY, _options.DSSTileCache, false);
                        FileInfo fi = new FileInfo(tempName);
                        if (fi.Length != 0 && fi.Length != 1033)
                        {
                            Bitmap temp = new Bitmap(tempName);

                            g.DrawImage(temp, new Rectangle(0, 0, 256, 256), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
                            temp.Dispose();
                        }
                        g.Dispose();
                        bmp.Save(filename, ImageFormat.Jpeg);
                        bmp.Dispose();
                    }
                }

                context.Response.WriteFile(filename);
                return; 
            }


            System.Net.WebClient client = new System.Net.WebClient();


            string url = String.Format("http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15", WWTUtil.GetServerID(tileX, tileY), WWTUtil.GetTileID(tileX, tileY, level, false));

            byte[] dat = client.DownloadData(url);


            client.Dispose();

            context.Response.OutputStream.Write(dat, 0, dat.Length);
        }
    }
}

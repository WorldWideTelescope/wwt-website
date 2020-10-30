using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class VeblendProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public VeblendProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = "";

            if (context.Request.Params["Q"] != null)
            {
                query = context.Request.Params["Q"];
            }
            else
            {
                context.Response.Write("No image");
                context.Response.End();
                return Task.CompletedTask;
            }

            string veKey = query;

            int level = 0;
            int tileX = 0;
            int tileY = 0;

            level = WWTUtil.GetTileAddressFromVEKey(veKey, out tileX, out tileY);


            string filename;
            string path;

            string DSSTileCache = _options.DSSTileCache;
            filename = String.Format(DSSTileCache + "\\VE\\level{0}\\{2}\\{1}_{2}.jpg", level, tileX, tileY);
            path = String.Format(DSSTileCache + "\\VE\\level{0}\\{2}", level, tileX, tileY);


            if (level > 20)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return Task.CompletedTask;
            }


            if (!File.Exists(filename))
            {
                if (level == 8 || level == 13)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    WWTUtil.DownloadVeTile(level, tileX, tileY, _options.DSSTileCache, true);

                    float[][] ptsArray ={
                                    new float[] {1, 0, 0, 0, 0},
                                    new float[] {0, 1, 0, 0, 0},
                                    new float[] {0, 0, 1, 0, 0},
                                    new float[] {0, 0, 0, 0.5f, 0},
                                    new float[] {0, 0, 0, 0, 1}};
                    ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                    ImageAttributes imgAttributes = new ImageAttributes();
                    imgAttributes.SetColorMatrix(clrMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Bitmap);
                    Bitmap bmp = new Bitmap(WWTUtil.DownloadVeTile(level, tileX, tileY, _options.DSSTileCache, true));
                    Graphics g = Graphics.FromImage(bmp);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    for (int y = 0; y < 2; y++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            string tempName = WWTUtil.DownloadVeTile(level + 1, tileX * 2 + x, tileY * 2 + y, _options.DSSTileCache, false);
                            FileInfo fi = new FileInfo(tempName);
                            if (fi.Length != 0 && fi.Length != 1033)
                            {
                                Bitmap temp = new Bitmap(tempName);

                                g.DrawImage(temp, new Rectangle(x * 128, y * 128, 128, 128), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
                            }
                        }
                    }
                    g.Dispose();
                    bmp.Save(filename, ImageFormat.Jpeg);
                    bmp.Dispose();
                }
                else
                {
                    WWTUtil.DownloadVeTile(level, tileX, tileY, _options.DSSTileCache, false);
                }
            }
            try
            {
                context.Response.WriteFile(filename);
            }
            catch
            {
            }
            context.Response.End();

            return Task.CompletedTask;
        }
    }
}

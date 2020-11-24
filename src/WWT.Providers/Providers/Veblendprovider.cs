#nullable disable

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/veblend.aspx")]
    public class VeblendProvider : RequestProvider
    {
        private readonly IVirtualEarthDownloader _veDownloader;

        public VeblendProvider(IVirtualEarthDownloader veDownloader)
        {
            _veDownloader = veDownloader;
        }

        public override string ContentType => ContentTypes.Jpeg;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            var query = context.Request.Params["Q"];

            if (query is null)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;
            }

            var level = _veDownloader.GetTileAddressFromVEKey(query, out var tileX, out var tileY);

            if (level > 20)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
                return;
            }

            if (level == 8 || level == 13)
            {
                var vepath = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level, tileX, tileY, token);

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

                using Bitmap bmp = new Bitmap(vepath);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    for (int y = 0; y < 2; y++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            using var veTile = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level + 1, tileX * 2 + x, tileY * 2 + y, token);

                            if (veTile.Length != 0 && veTile.Length != 1033)
                            {
                                using var temp = new Bitmap(veTile);

                                g.DrawImage(temp, new Rectangle(x * 128, y * 128, 128, 128), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
                            }
                        }
                    }
                }

                bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
            else
            {
                using var veTile = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level, tileX, tileY, token);
                await veTile.CopyToAsync(context.Response.OutputStream);
            }
        }
    }
}

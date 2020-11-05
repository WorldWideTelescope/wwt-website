using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class TileImageProvider : RequestProvider
    {
        private readonly IFileNameHasher _hasher;
        private readonly HttpClient _httpClient;
        private readonly ITileAccessor _tileAccessor;
        private readonly string _tempDir;

        public TileImageProvider(IFileNameHasher hasher, ITileAccessor tileAccessor)
        {
            _hasher = hasher;
            _httpClient = new HttpClient();
            _tileAccessor = tileAccessor;
            _tempDir = Path.Combine(Path.GetTempPath(), "wwt_temp");

            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }
        }

        public override string ContentType => ContentTypes.XWtml;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            {
                if (context.Request.Params["debug"] != null)
                {
                    context.Response.ClearHeaders();
                    context.Response.ContentType = "text/plain";

                }

                string url = "";
                if (context.Request.Params["imageurl"] != null)
                {
                    url = context.Request.Params["imageurl"];
                }

                if (string.IsNullOrEmpty(url))
                {
                    url = "http://www.spitzer.caltech.edu/uploaded_files/images/0009/0848/sig12-011.jpg";
                }

                var id = _hasher.HashName(url).ToString();
                var filename = await DownloadFileAsync(url, id, token);
                var wcsImage = WcsImage.FromFile(filename);

                if (wcsImage != null)
                {
                    var creator = _tileAccessor.CreateTile(id);

                    if (!await creator.ExistsAsync(token))
                    {
                        using var bmp = wcsImage.GetBitmap();
                        wcsImage.AdjustScale(bmp.Width, bmp.Height);
                        await MakeThumbnailAsync(bmp, creator, token);
                        await TileBitmap(bmp, creator, token);
                    }

                    string name = wcsImage.Keywords[0];
                    bool reverseparity = false;
                    string creditsUrl = wcsImage.CreditsUrl;
                    string credits = wcsImage.Copyright;
                    string thumb = "http://www.worldwidetelescope.org/wwtweb/tilethumb.aspx?name=" + id;
                    double rotation = wcsImage.Rotation;

                    int maxLevels = CalcMaxLevels((int)wcsImage.SizeX, (int)wcsImage.SizeY);
                    double scale = wcsImage.ScaleY * Math.Pow(2, maxLevels) * 256;
                    double y = 0;
                    double x = 0;
                    double dec = wcsImage.CenterY;
                    double ra = wcsImage.CenterX;

                    if (context.Request.Params["debug"] != null)
                    {
                        name = context.Request.Params["name"];
                        name = name.Replace(",", "");
                    }

                    if (context.Request.Params["ra"] != null)
                    {
                        ra = Math.Max(0, Math.Min(360.0, Convert.ToDouble(context.Request.Params["ra"])));
                    }

                    if (context.Request.Params["dec"] != null)
                    {
                        dec = Math.Max(-90, Math.Min(90, Convert.ToDouble(context.Request.Params["dec"])));
                    }

                    if (context.Request.Params["x"] != null)
                    {
                        x = Convert.ToDouble(context.Request.Params["x"]);
                    }

                    if (context.Request.Params["y"] != null)
                    {
                        y = Convert.ToDouble(context.Request.Params["y"]);
                    }

                    if (context.Request.Params["scale"] != null)
                    {
                        scale = Convert.ToDouble(context.Request.Params["scale"]) * Math.Pow(2, maxLevels) * 256;
                    }

                    if (context.Request.Params["rotation"] != null)
                    {
                        rotation = Convert.ToDouble(context.Request.Params["rotation"]) - 180;
                    }

                    if (context.Request.Params["thumb"] != null)
                    {
                        thumb = context.Request.Params["thumb"];
                    }

                    if (context.Request.Params["credits"] != null)
                    {
                        credits = context.Request.Params["credits"];
                    }

                    if (context.Request.Params["creditsUrl"] != null)
                    {
                        creditsUrl = context.Request.Params["creditsUrl"];
                    }

                    if (context.Request.Params["reverseparity"] != null)
                    {
                        reverseparity = Convert.ToBoolean(context.Request.Params["reverseparity"]);
                    }

                    if (context.Request.Params["goto"] != null)
                    {
                        bool bgoto = Convert.ToBoolean(context.Request.Params["goto"]);
                    }

                    if (scale == 0)
                    {
                        scale = .1;
                    }

                    double zoom = scale * 4;

                    string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Name=\"{0}\" Group=\"{14}\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\" Opacity=\"100\" Thumbnail=\"{10}\" Constellation=\"\">\n <ForegroundImageSet>\n <ImageSet DataSetType=\"Sky\" Name=\"{0}\" BandPass=\"Visible\" Url=\"http://www.worldwidetelescope.org/wwtweb/GetTile.aspx?q={{1}},{{2}},{{3}},{8}\" TileLevels=\"{15}\" WidthFactor=\"1\" Rotation=\"{5}\" Projection=\"Tan\" FileType=\".png\" CenterY=\"{2}\" CenterX=\"{9}\" BottomsUp=\"{13}\" OffsetX=\"{6}\" OffsetY=\"{7}\" BaseTileLevel=\"0\" BaseDegreesPerTile=\"{4}\">\n<Credits>{11}</Credits>\n<CreditsUrl>{12}</CreditsUrl>\n<ThumbnailUrl>{10}</ThumbnailUrl>\n</ImageSet>\n</ForegroundImageSet>\n</Place>\n</Folder>", name, ra / 15, dec, zoom, scale, rotation, x, y, id, ra, thumb, credits, creditsUrl, reverseparity, "Explorer", maxLevels);

                    context.Response.Write(xml);
                }
            }
        }

        public async Task TileBitmap(Bitmap bmp, ITileCreator creator, CancellationToken token)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            double aspect = (double)width / (double)height;

            //narrower
            int levels = 1;
            int maxHeight = 256;
            int maxWidth = 512;

            do
            {
                if (aspect < 2)
                {
                    if (maxHeight >= height)
                    {
                        break;
                    }
                }
                else
                {
                    if (maxWidth >= width)
                    {
                        break;
                    }
                }
                levels++;
                maxHeight *= 2;
                maxWidth *= 2;
            } while (true);

            int xOffset = (maxWidth - width) / 2;
            int yOffset = (maxHeight - height) / 2;

            int l = levels;

            int gridX = 256;
            int gridY = 256;
            while (l > 0)
            {
                l--;
                int currentLevel = l;

                int tilesX = 2 * (int)Math.Pow(2, l);
                int tilesY = (int)Math.Pow(2, l);

                for (int y = 0; y < tilesY; y++)
                {
                    for (int x = 0; x < tilesX; x++)
                    {
                        if ((((x + 1) * gridX) > xOffset) && (((y + 1) * gridX) > yOffset) &&
                            (((x) * gridX) < (xOffset + width)) && (((y) * gridX) < (yOffset + height)))
                        {
                            using Bitmap bmpTile = new Bitmap(256, 256);
                            using (Graphics gfx = Graphics.FromImage(bmpTile))
                            {
                                gfx.DrawImage(bmp, new Rectangle(0, 0, 256, 256),
                                    new Rectangle((x * gridX) - xOffset, (y * gridX) - yOffset, gridX, gridX),
                                    GraphicsUnit.Pixel);
                            }

                            using var stream = bmpTile.SaveToStream(ImageFormat.Png);
                            await creator.AddTileAsync(stream, currentLevel, x, y, token);
                        }
                    }
                }

                gridX *= 2;
                gridY *= 2;
            }
        }

        public async Task MakeThumbnailAsync(Bitmap imgOrig, ITileCreator creator, CancellationToken token)
        {
            using var stream = CreateThumbnailStream(imgOrig);

            await creator.AddThumbnailAsync(stream, token);
        }

        private Stream CreateThumbnailStream(Bitmap imgOrig)
        {
            try
            {
                using Bitmap bmpThumb = new Bitmap(96, 45);

                using (Graphics g = Graphics.FromImage(bmpThumb))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    double imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                    double clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                    int cw = bmpThumb.Width;
                    int ch = bmpThumb.Height;

                    if (imageAspect < clientAspect)
                    {
                        ch = (int)((double)cw / imageAspect);
                    }
                    else
                    {
                        cw = (int)((double)ch * imageAspect);
                    }

                    int cx = (bmpThumb.Width - cw) / 2;
                    int cy = ((bmpThumb.Height - ch) / 2); // - 1;
                    Rectangle destRect = new Rectangle(cx, cy, cw, ch); //+ 1);

                    Rectangle srcRect = new Rectangle(0, 0, imgOrig.Width, imgOrig.Height);
                    g.DrawImage(imgOrig, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                }
                return bmpThumb.SaveToStream(ImageFormat.Jpeg);
            }
            catch
            {
                using Bitmap bmp = new Bitmap(96, 45);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Blue);
                }

                return bmp.SaveToStream(ImageFormat.Jpeg);
            }
        }

        public int CalcMaxLevels(int SizeX, int SizeY)
        {
            int levels = 0;
            int maxHeight = 256;
            int maxWidth = 512;
            double aspect = (double)SizeX / (double)SizeY;

            do
            {
                if (aspect < 2)
                {
                    if (maxHeight >= SizeY)
                    {
                        break;
                    }
                }
                else
                {
                    if (maxWidth >= SizeX)
                    {
                        break;
                    }
                }
                levels++;
                maxHeight *= 2;
                maxWidth *= 2;
            } while (true);

            return levels;
        }

        private async Task<string> DownloadFileAsync(string url, string id, CancellationToken token)
        {
            var path = Path.Combine(_tempDir, id);

            if (File.Exists(path))
            {
                return path;
            }

            using var stream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);

            using (var fs = File.OpenWrite(path))
            {
                await stream.CopyToAsync(fs, token).ConfigureAwait(false);
            }

            return path;
        }
    }
}

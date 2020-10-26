using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class TileImageProvider : TileImage
    {
        private readonly IFileNameHasher _hasher;

        public TileImageProvider(IFileNameHasher hasher)
        {
            _hasher = hasher;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            {
                //if (context.Request.Cookies["alphakey"] != null && context.Request.Params["wtml"] == null)
                // if (context.Request.Cookies["fullclient"] == null && context.Request.Params["wtml"] == null)
                // {
                //context.Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?wtml="+HttpUtility.UrlEncode(context.Request.Url.ToString().Replace(",","-") +"&wtml=true"));
                //return;
                //}



                if (context.Request.Params["debug"] != null)
                {
                    context.Response.ClearHeaders();
                    context.Response.ContentType = "text/plain";

                }

                string url = "";
                bool bgoto = false;
                bool reverseparity = false;
                string creditsUrl = "";
                string credits = "";
                string thumb = "";
                double rotation = 1.0;

                double scale = 1.0;
                double y = 0;
                double x = 0;
                double dec = 0;
                double ra = 0;
                string name = "";
                int maxLevels = 1;

                if (context.Request.Params["imageurl"] != null)
                {
                    url = context.Request.Params["imageurl"];
                }

                if (String.IsNullOrEmpty(url))
                {
                    url = "http://www.spitzer.caltech.edu/uploaded_files/images/0009/0848/sig12-011.jpg";
                }

                int hashID = _hasher.HashName(url);

                //hashID = 12345;
                string path = ConfigurationManager.AppSettings["DSSTileCache"] + "\\imagesTiler\\dowloadImages\\";

                string filename = path + hashID + ".png";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(filename))
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(url, filename);
                }


                WcsImage wcsImage = WcsImage.FromFile(filename);

                if (wcsImage != null)
                {
                    bool hasAvm = wcsImage.ValidWcs;


                    Bitmap bmp = wcsImage.GetBitmap();
                    wcsImage.AdjustScale(bmp.Width, bmp.Height);

                    MakeThumbnail(bmp, hashID.ToString());

                    name = wcsImage.Keywords[0];
                    reverseparity = false;
                    creditsUrl = wcsImage.CreditsUrl;
                    credits = wcsImage.Copyright;
                    thumb = "http://www.worldwidetelescope.org/wwtweb/tilethumb.aspx?name=" + hashID;
                    rotation = wcsImage.Rotation;

                    maxLevels = CalcMaxLevels((int)wcsImage.SizeX, (int)wcsImage.SizeY);
                    scale = wcsImage.ScaleY * Math.Pow(2, maxLevels) * 256;
                    y = 0;
                    x = 0;
                    dec = wcsImage.CenterY;
                    ra = wcsImage.CenterX;

                    //if (tileIt)
                    {
                        TileBitmap(bmp, hashID.ToString());
                    }

                    // todo make thumbnail
                    //pl.ThumbNail = UiTools.MakeThumbnail(bmp);


                    bmp.Dispose();
                    GC.SuppressFinalize(bmp);
                    bmp = null;

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
                        bgoto = Convert.ToBoolean(context.Request.Params["goto"]);
                    }

                    if (scale == 0)
                    {
                        scale = .1;
                    }
                    double zoom = scale * 4;

                    //scale = scale / 3600.0;
                    //bgoto = true;

                    string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Name=\"{0}\" Group=\"{14}\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\" Opacity=\"100\" Thumbnail=\"{10}\" Constellation=\"\">\n <ForegroundImageSet>\n <ImageSet DataSetType=\"Sky\" Name=\"{0}\" BandPass=\"Visible\" Url=\"http://www.worldwidetelescope.org/wwtweb/GetTile.aspx?q={{1}},{{2}},{{3}},{8}\" TileLevels=\"{15}\" WidthFactor=\"1\" Rotation=\"{5}\" Projection=\"Tan\" FileType=\".png\" CenterY=\"{2}\" CenterX=\"{9}\" BottomsUp=\"{13}\" OffsetX=\"{6}\" OffsetY=\"{7}\" BaseTileLevel=\"0\" BaseDegreesPerTile=\"{4}\">\n<Credits>{11}</Credits>\n<CreditsUrl>{12}</CreditsUrl>\n<ThumbnailUrl>{10}</ThumbnailUrl>\n</ImageSet>\n</ForegroundImageSet>\n</Place>\n</Folder>", name, ra / 15, dec, zoom, scale, rotation, x, y, hashID, ra, thumb, credits, creditsUrl, reverseparity, "Explorer", maxLevels);

                    context.Response.Write(xml);
                }

                return Task.CompletedTask;
            }
        }
    }
}

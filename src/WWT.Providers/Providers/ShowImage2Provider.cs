using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WWT.Providers
{
    public class ShowImage2Provider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (context.Request.ContainsCookie("alphakey"))
            {
                context.Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?Wtml=" + HttpUtility.UrlEncode(context.Request.Url.ToString() + "&wtml=true"));
                return Task.CompletedTask;
            }

            if (context.Request.Params["debug"] != null)
            {
                context.Response.ClearHeaders();
                context.Response.ContentType = "text/plain";

            }

            string name = context.Request.Params["name"];
            double ra = 0;
            if (context.Request.Params["ra"] != null)
            {
                ra = Math.Max(0, Math.Min(360.0, Convert.ToDouble(context.Request.Params["ra"])));
            }

            double dec = 0;
            if (context.Request.Params["dec"] != null)
            {
                dec = Math.Max(-90, Math.Min(90, Convert.ToDouble(context.Request.Params["dec"])));
            }

            double x = 0;
            if (context.Request.Params["x"] != null)
            {
                x = Convert.ToDouble(context.Request.Params["x"]);
            }

            double y = 0;
            if (context.Request.Params["y"] != null)
            {
                y = Convert.ToDouble(context.Request.Params["y"]);
            }

            double scale = 1.0;
            if (context.Request.Params["scale"] != null)
            {
                scale = Convert.ToDouble(context.Request.Params["scale"]);
            }



            double rotation = 1.0;
            if (context.Request.Params["rotation"] != null)
            {
                rotation = Convert.ToDouble(context.Request.Params["rotation"]) - 180;
            }

            string url = "";
            if (context.Request.Params["imageurl"] != null)
            {
                url = context.Request.Params["imageurl"];
            }

            string thumb = "";
            if (context.Request.Params["thumb"] != null)
            {
                thumb = context.Request.Params["thumb"];
            }

            string credits = "";
            if (context.Request.Params["credits"] != null)
            {
                credits = context.Request.Params["credits"];
            }

            string creditsUrl = "";
            if (context.Request.Params["creditsUrl"] != null)
            {
                creditsUrl = context.Request.Params["creditsUrl"];
            }

            bool reverseparity = false;
            if (context.Request.Params["reverseparity"] != null)
            {
                reverseparity = Convert.ToBoolean(context.Request.Params["reverseparity"]);
            }

            bool bgoto = false;
            if (context.Request.Params["goto"] != null)
            {
                bgoto = Convert.ToBoolean(context.Request.Params["goto"]);
            }

            double zoom = scale * y / 360;
            scale = scale / 3600.0;
            //string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Group=\"Goto\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\"/>\n</Folder>", name, ra, dec, zoom);
            string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Name=\"{0}\" Group=\"{14}\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\" Opacity=\"100\" Thumbnail=\"{10}\" Constellation=\"\">\n <ForegroundImageSet>\n <ImageSet DataSetType=\"Sky\" BandPass=\"Visible\" Url=\"{8}\" TileLevels=\"0\" WidthFactor=\"2\" Rotation=\"{5}\" Projection=\"SkyImage\" FileType=\".tif\" CenterY=\"{2}\" CenterX=\"{9}\" BottomsUp=\"{13}\" OffsetX=\"{6}\" OffsetY=\"{7}\" BaseTileLevel=\"0\" BaseDegreesPerTile=\"{4}\">\n<Credits>{11}</Credits>\n<CreditsUrl>{12}</CreditsUrl>\n</ImageSet>\n</ForegroundImageSet>\n</Place>\n</Folder>", name, ra / 15, dec, zoom, scale, rotation, x, y, url, ra, thumb, credits, creditsUrl, reverseparity, bgoto ? "Goto" : "Search");

            context.Response.Write(xml);

            return Task.CompletedTask;
        }
    }
}

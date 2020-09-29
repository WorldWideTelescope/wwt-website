using System;
using System.Web;

namespace WWT.Providers
{
    public class GotoProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            //if (context.Request.Cookies["alphakey"] != null && context.Request.Params["wtml"] == null)
            if (context.Request.Cookies["fullclient"] == null && context.Request.Params["wtml"] == null)
            {
                context.Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?wtml=" + HttpUtility.UrlEncode(context.Request.Url.ToString() + "&wtml=true"));
                return;
            }

            string name = context.Request.Params["object"];
            double ra = 0;
            if (context.Request.Params["ra"] != null)
            {
                ra = Math.Max(0, Math.Min(24.0, Convert.ToDouble(context.Request.Params["ra"])));
            }
            double dec = 0;
            if (context.Request.Params["dec"] != null)
            {
                dec = Math.Max(-90, Math.Min(90, Convert.ToDouble(context.Request.Params["dec"])));
            }
            double zoom = .25;
            if (context.Request.Params["zoom"] != null)
            {
                zoom = Math.Max(0.001373291015625, Math.Min(360, Convert.ToDouble(context.Request.Params["zoom"])));
            }

            string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Group=\"Goto\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\"/>\n</Folder>", name, ra, dec, zoom);
            context.Response.Write(xml);
        }
    }
}

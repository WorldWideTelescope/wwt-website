#nullable disable

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Goto.aspx")]
    public class GotoProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.XWtml;

        private readonly IExternalUrlInfo _urlInfo;

        public GotoProvider(IExternalUrlInfo urlInfo)
        {
            _urlInfo = urlInfo;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (context.Request.Params["wtml"] == null)
            {
                // If we're called with no "wtml" parameter, we redirect the
                // caller to the webclient, with a WTML argument pointing to the
                // current request URL, now with "wtml" set to true. This will
                // cause us to emit WTML that the webclient will then parse.
                var gb = new UriBuilder(_urlInfo.GetExternalRequestUrl(context.Request));
                if (String.IsNullOrEmpty(gb.Query))
                    gb.Query = "wtml=true";
                else
                    gb.Query = gb.Query.Substring(1) + "&wtml=true";

                var wb = _urlInfo.GetWebclientBuilder(context.Request);
                wb.Query = "wtml=" + WebUtility.UrlEncode(gb.ToString());
                context.Response.Redirect(wb.ToString());
                return Task.CompletedTask;
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

            return context.Response.WriteAsync(xml, token);
        }
    }
}

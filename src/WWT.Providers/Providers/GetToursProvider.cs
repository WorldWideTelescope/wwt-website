using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WWT.Providers
{
    public class GetToursProvider : GetTours
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string etag = context.Request.Headers["If-None-Match"];

            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "application/x-wtml";

            string toursXML = null;
            UpdateCacheEx();
            toursXML = (string)HttpContext.Current.Cache["WWTXMLTours"];

            if (toursXML != null)
            {
                int version = (int)HttpContext.Current.Cache.Get("Version");
                string newEtag = version.ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", newEtag);
                    context.Response.AddHeader("Cache-Control", "no-cache");
                    context.Response.Write(toursXML);
                }
                else
                {
                    context.Response.Status = "304 Not Modified";
                }
            }
            context.Response.End();

            return Task.CompletedTask;
        }
    }
}

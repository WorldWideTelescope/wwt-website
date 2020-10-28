using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class Catalog2Provider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string etag = context.Request.Headers["If-None-Match"];
            string filename = "";
            string webDir = Path.Combine(ConfigurationManager.AppSettings["DataDir"], "data");

            if (context.Request.Params["Q"] != null)
            {
                string query = context.Request.Params["Q"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(webDir, query + ".txt");
            }
            else if (context.Request.Params["X"] != null)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/xml";
                string query = context.Request.Params["X"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(webDir, query + ".xml");
            }
            else if (context.Request.Params["W"] != null)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/xml";

                string query = context.Request.Params["W"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");

                filename = Path.Combine(webDir, query + ".wtml");
            }

            if (!string.IsNullOrEmpty(filename))
            {
                var fi = new FileInfo(filename);
                fi.LastWriteTimeUtc.ToString();

                string newEtag = fi.LastWriteTimeUtc.ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", fi.LastWriteTimeUtc.ToString());
                    context.Response.AddHeader("Cache-Control", "no-cache");
                    context.Response.WriteFile(filename);
                }
                else
                {
                    context.Response.Status = "304 Not Modified";
                }
            }

            return Task.CompletedTask;
        }
    }
}

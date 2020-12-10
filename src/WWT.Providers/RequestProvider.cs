using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public abstract class RequestProvider
    {
        public abstract Task RunAsync(IWwtContext context, CancellationToken token);

        public virtual bool IsCacheable => true;

        public abstract string ContentType { get; }

        protected static class ContentTypes
        {
            public const string Png = "image/png";

            public const string Xml = "text/xml";

            public const string Jpeg = "image/jpeg";

            public const string OctetStream = "application/octet-stream";

            public const string Text = "text/plain";

            public const string Html = "text/html";

            public const string XWtt = "application/x-wtt";

            public const string XWtml = "application/x-wtml";

            public const string Zip = "application/zip";
        }

        protected Task Report404Async(IWwtContext context, string detail, CancellationToken token) {
            context.Response.StatusCode = 404;
            context.Response.ContentType = ContentTypes.Text;
            return context.Response.WriteAsync($"HTTP/404 Not Found\n\n{detail}", token);
        }

        // This function is async because it handles the case of reporting an
        // error when there's an issue with the "Q" query parameter. In the
        // happy path it doesn't do any I/O.
        protected async Task<(bool, int, int, int)> HandleLXYQParameter(IWwtContext context, CancellationToken token) {
            try {
                string query = context.Request.Params["Q"];
                string[] values = query.Split(',');
                int level = Convert.ToInt32(values[0]);
                int tileX = Convert.ToInt32(values[1]);
                int tileY = Convert.ToInt32(values[2]);
                return (false, level, tileX, tileY);
            } catch {
                context.Response.StatusCode = 400;
                context.Response.ContentType = ContentTypes.Text;
                await context.Response.WriteAsync("HTTP/400 illegal Q parameter", token);
                context.Response.Flush();
                context.Response.End();
                return (true, 0, 0, 0);
            }
        }

        protected async Task<(bool, int, int, int, string)> HandleLXYExtraQParameter(IWwtContext context, CancellationToken token) {
            try {
                string query = context.Request.Params["Q"];
                string[] values = query.Split(',');
                int level = Convert.ToInt32(values[0]);
                int tileX = Convert.ToInt32(values[1]);
                int tileY = Convert.ToInt32(values[2]);
                string extra = "";

                if (values.Length > 3)
                    extra = values[3];

                return (false, level, tileX, tileY, extra);
            } catch {
                context.Response.StatusCode = 400;
                context.Response.ContentType = ContentTypes.Text;
                await context.Response.WriteAsync("HTTP/400 illegal Q parameter", token);
                context.Response.Flush();
                context.Response.End();
                return (true, 0, 0, 0, "");
            }
        }
    }
}

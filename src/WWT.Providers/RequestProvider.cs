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
        }
    }
}

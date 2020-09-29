using System.Web;
using System.Web.UI;

namespace WWT.Providers
{
    internal class PageWwtContext : IWwtContext
    {
        public PageWwtContext(Page page)
        {
            Request = new HttpRequestWrapper(page.Request);
            Response = new HttpResponseWrapper(page.Response);
            Server = new HttpServerUtilityWrapper(page.Server);
        }

        public HttpRequestBase Request { get; }

        public HttpResponseBase Response { get; }

        public HttpServerUtilityBase Server { get; }
    }
}

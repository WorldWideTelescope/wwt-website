using System.Web;
using System.Web.UI;

namespace WWT.Providers
{
    public readonly struct WwtContext
    {
        private readonly Page _page;

        public WwtContext(Page page)
        {
            _page = page;
        }

        public HttpRequest Request => _page.Request;

        public HttpResponse Response => _page.Response;

        public HttpServerUtility Server => _page.Server;
    }
}

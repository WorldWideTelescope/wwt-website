using System.Web;

namespace WWT.Providers
{
    public readonly struct WwtContext
    {
        public WwtContext(HttpRequest request, HttpResponse response)
        {
            Request = request;
            Response = response;
        }

        public HttpRequest Request { get; }

        public HttpResponse Response { get; }
    }
}

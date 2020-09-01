using System.Dynamic;
using System.Web;

namespace WWT.Providers
{
    public abstract class RequestProvider
    {
        public void Run(HttpRequest request, HttpResponse response)
            => Run(new WwtContext(request, response));

        public abstract void Run(WwtContext context);

        public static RequestProvider Get<TProvider>()
            where TProvider : RequestProvider, new()
            => new TProvider();
    }
}

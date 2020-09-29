using System.Web;

namespace WWT.Providers
{
    public interface IWwtContext
    {
        HttpRequestBase Request { get; }

        HttpResponseBase Response { get; }

        HttpServerUtilityBase Server { get; }
    }
}

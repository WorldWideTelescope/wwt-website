using System;

namespace WWT.Providers
{
    public interface IExternalUrlInfo
    {
        // Get the URL of the request as understood by the outside world. This
        // is different than IRequest.Url because we live behind an App Gateway
        // that must change the Host header to allow for vhost-based routing
        // behind the gateway. We "correct" IRequest.Url by using a configured
        // map from behind-the-gateway hostnames to public-facing hostnames.
        Uri GetExternalRequestUrl(IRequest request);

        // Get the baseurl of this server as understood by the outside world.
        // An example might be "https://worldwidetelescope.org/".
        Uri GetExternalBaseUrl(IRequest request);

        // Get a UriBuilder for referring to the WWT web client
        UriBuilder GetWebclientBuilder(IRequest request);
    }
}

using System;
using System.Collections.Generic;

namespace WWT.Providers
{
    // Constructing our URLs for the outside world. Needed since this server may
    // be running behind a gateway such that its hostname is unrelated to what
    // we want to expose.
    //
    // The way that Azure deployment slots seem to work, our staging and
    // production apps have to share identical settings if we want to use its
    // "swap" functionality -- so we can't just hardcode a single URL. So we
    // have a variable that maps from HTTP host to base-URL, in the form
    // "host1=http://url1,host2=http://url1".
    public class WwtExternalUrlInfo : IExternalUrlInfo
    {
        private Dictionary<string, string> _authorityMap;

        public WwtExternalUrlInfo(WwtOptions options)
        {
            _authorityMap = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(options.ExternalUrlMapText)) {
                foreach (var item in options.ExternalUrlMapText.Split(',')) {
                    var pieces = item.Split(new char[] { '=' }, 2);
                    var authorityIn = pieces[0].ToLower();
                    var hostOut = pieces[1];
                    _authorityMap[authorityIn] = hostOut;
                }
            }
        }

        public Uri GetExternalRequestUrl(IRequest request)
        {
            var internalUrl = request.Url;
            var ub = new UriBuilder(internalUrl);
            var authIn = internalUrl.Authority.ToLower();

            if (_authorityMap.ContainsKey(authIn)) {
                ub.Host = _authorityMap[authIn];
                ub.Port = -1; // force this to default
            }

            return ub.Uri;
        }

        public Uri GetExternalBaseUrl(IRequest request)
        {
            var internalUrl = request.Url;
            var ub = new UriBuilder(internalUrl);
            var authIn = internalUrl.Authority.ToLower();

            if (_authorityMap.ContainsKey(authIn)) {
                ub.Host = _authorityMap[authIn];
                ub.Port = -1; // force this to default
            }

            ub.Path = "/";
            ub.Query = null;
            ub.Fragment = null;
            return ub.Uri;
        }

        public UriBuilder GetWebclientBuilder(IRequest request)
        {
            // One day we might bother to make this configurable, but not today.
            var scheme = request.Url.Scheme;
            var ub = new UriBuilder($"{scheme}://worldwidetelescope.org/webclient/");
            ub.Port = -1; // avoid ":80" in default stringification
            return ub;
        }
    }
}

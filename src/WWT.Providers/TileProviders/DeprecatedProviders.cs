// This provider returns "gone" messages for broken endpoints that we've
// deprecated.
//
// If it turns out that we need any of these again, you'll have to look into the
// Git history and see how the endpoint used to be implemented. However, many of
// these have been non-functional for a long time.

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/BingDemTile2.aspx")]
    [RequestEndpoint("/wwtweb/dem.aspx")]
    [RequestEndpoint("/wwtweb/dembath.aspx")]
    [RequestEndpoint("/wwtweb/demmars_new.aspx")]
    [RequestEndpoint("/wwtweb/DSSToast.aspx")]
    [RequestEndpoint("/wwtweb/DustToast.aspx")]
    [RequestEndpoint("/wwtweb/EarthBlend.aspx")]
    [RequestEndpoint("/wwtweb/EarthMerBath.aspx")]
    [RequestEndpoint("/wwtweb/hAlphaToast.aspx")]
    [RequestEndpoint("/wwtweb/Hirise.aspx")]
    [RequestEndpoint("/wwtweb/HiriseDem2.aspx")]
    [RequestEndpoint("/wwtweb/HiriseDem3.aspx")]
    [RequestEndpoint("/wwtweb/HiriseDem.aspx")]
    [RequestEndpoint("/wwtweb/jupiter.aspx")]
    [RequestEndpoint("/wwtweb/mandel1.aspx")]
    [RequestEndpoint("/wwtweb/mars.aspx")]
    [RequestEndpoint("/wwtweb/MartianTile2.aspx")]
    [RequestEndpoint("/wwtweb/moondem.aspx")]
    [RequestEndpoint("/wwtweb/moonOct.aspx")]
    [RequestEndpoint("/wwtweb/RassToast.aspx")]
    [RequestEndpoint("/wwtweb/SDSSToast2.aspx")]
    [RequestEndpoint("/wwtweb/TychoOct.aspx")]
    [RequestEndpoint("/wwtweb/veblend.aspx")]
    [RequestEndpoint("/wwtweb/vlssToast.aspx")]
    [RequestEndpoint("/wwtweb/wmap.aspx")]
    public class DeprecatedProvider : RequestProvider
    {
        private readonly ILogger<CatalogProvider> _logger;

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => true;

        public DeprecatedProvider(ILogger<CatalogProvider> logger)
        {
            _logger = logger;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            _logger.LogWarning("request to deprecated endpoint {url}", context.Request.Url.ToString());
            context.Response.StatusCode = 410;
            await context.Response.WriteAsync("HTTP/410 Gone\n\nThis endpoint is no longer supported.\nFile an issue at https://github.com/WorldWideTelescope/wwt-website/issues if you still need it.\n", token);
            context.Response.Flush();
            context.Response.End();
        }
    }
}

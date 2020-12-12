#nullable disable

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

using WWT.Catalog;
using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/login.aspx")]
    public class LoginProvider : LoginUser
    {
        private readonly ICatalogAccessor _catalog;
        private readonly ILogger<CatalogProvider> _logger;

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public LoginProvider(ICatalogAccessor catalogAccessor, ILogger<CatalogProvider> logger)
        {
            _catalog = catalogAccessor;
            _logger = logger;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            var catalog_file = "wwt2_login.txt";

            if (context.Request.Params["Equinox"] == null) {
                // The pre-equinox file combined ClientVersion.txt, dataversion.txt,
                // message.txt, warnver.txt, minver.txt, and updateurl.txt.
                catalog_file = "wwt2_login_pre_equinox.txt";
            }

            context.Response.AddHeader("Expires", "0");

            var catalogEntry = await _catalog.GetCatalogEntryAsync(catalog_file, token);

            if (catalogEntry is null)
            {
                _logger.LogError(string.Format("wwt2::{0} file mising from backing storage", catalog_file));
                context.Response.StatusCode = 500;
                return;
            }

            using (var c = catalogEntry.Contents)
            {
                await c.CopyToAsync(context.Response.OutputStream, token);
                context.Response.Flush();
                context.Response.End();
            }
        }
    }
}

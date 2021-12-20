#nullable disable

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

using WWT.Catalog;
using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/login6.aspx")]
    public class Login6Provider : LoginUser
    {
        private readonly ICatalogAccessor _catalog;
        private readonly ILogger<CatalogProvider> _logger;

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public Login6Provider(ICatalogAccessor catalogAccessor, ILogger<CatalogProvider> logger)
        {
            _catalog = catalogAccessor;
            _logger = logger;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            var catalog_file = "wwt6_login.txt";

            context.Response.AddHeader("Expires", "0");

            var catalogEntry = await _catalog.GetCatalogEntryAsync(catalog_file, token);

            if (catalogEntry is null)
            {
                _logger.LogError(string.Format("wwt6::{0} file missing from backing storage", catalog_file));
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

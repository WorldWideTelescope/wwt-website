#nullable disable

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WWT.Catalog;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/ExcelAddinUpdate.aspx")]
    public class ExcelAddinUpdateProvider : RequestProvider
    {
        private readonly ICatalogAccessor _catalog;
        private readonly ILogger<CatalogProvider> _logger;

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public ExcelAddinUpdateProvider(ICatalogAccessor catalogAccessor, ILogger<CatalogProvider> logger)
        {
            _catalog = catalogAccessor;
            _logger = logger;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            var catalogEntry = await _catalog.GetCatalogEntryAsync("wwt2_exceladdin.txt", token);

            if (catalogEntry is null)
            {
                _logger.LogError("wwt2::exceladdin file mising from backing storage");
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

using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.Catalog;
using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Catalog.aspx")]
    public class CatalogProvider : RequestProvider
    {
        private readonly ICatalogAccessor _catalog;
        private readonly ILogger<CatalogProvider> _logger;

        public CatalogProvider(ICatalogAccessor catalogAccessor, ILogger<CatalogProvider> logger)
        {
            _catalog = catalogAccessor;
            _logger = logger;
        }

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = "";
            string extension = "";

            if (context.Request.Params["Q"] != null)
            {
                query = context.Request.Params["Q"];
                extension = "txt";
            }
            else if (context.Request.Params["X"] != null)
            {
                query = context.Request.Params["X"];
                extension = "xml";
            }
            else if (context.Request.Params["W"] != null)
            {
                query = context.Request.Params["W"];
                extension = "wtml";
            }
            else
            {
                await Report400Async(context, "must pass Q or X or W query parameter", token);
                return;
            }

            query = query.Replace("..", "").Replace("\\", "").Replace("/", "");
            string filename = $"{query}.{extension}";

            var catalogEntry = await _catalog.GetCatalogEntryAsync(filename, token);
            if (catalogEntry is null)
            {
                string msg = $"Requested catalog item {filename} does not exist.";
                _logger.LogWarning(msg);
                await Report404Async(context, msg, token);
                return;
            }

            string mtime = catalogEntry.LastModified.ToUniversalTime().ToString();
            string etag = $"\"{mtime}\"";

            using (var c = catalogEntry.Contents)
            {
                await context.Response.ServeStreamAsync(c, "text/plain", etag);
            }
        }
    }
}

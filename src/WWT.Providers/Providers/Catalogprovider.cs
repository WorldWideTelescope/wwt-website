#nullable disable

using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Catalog;

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
            string etag = context.Request.Headers["If-None-Match"];
            string filename = "";

            if (context.Request.Params["Q"] != null)
            {
                string query = context.Request.Params["Q"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(query + ".txt");
            }
            else if (context.Request.Params["X"] != null)
            {
                string query = context.Request.Params["X"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = $"{query}.xml";
            }
            else if (context.Request.Params["W"] != null)
            {
                string query = context.Request.Params["W"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = $"{query}.wtml";
            }

            if (!await GetEntry(context, etag, filename, token))
            {
                context.Response.StatusCode = 404;
            }
        }

        private async Task<bool> GetEntry(IWwtContext context, string etag, string filename, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            var catalogEntry = await _catalog.GetCatalogEntryAsync(filename, token);

            if (catalogEntry is null)
            {
                _logger.LogWarning("Requested catalog {Name} does not exist.", filename);
                return false;
            }

            string newEtag = catalogEntry.LastModified.ToUniversalTime().ToString();

            if (newEtag != etag)
            {
                context.Response.AddHeader("etag", newEtag);

                using (var c = catalogEntry.Contents)
                {
                    await c.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                await Report304Async(context, token);
            }

            return true;
        }
    }
}

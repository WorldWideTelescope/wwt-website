using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Catalog;

namespace WWT.Providers
{
    public class CatalogProvider : RequestProvider
    {
        private readonly FilePathOptions _options;
        private readonly ICatalogAccessor _catalog;        
        private readonly bool _useXmlContentType;

        public CatalogProvider(FilePathOptions options, ICatalogAccessor catalogAccessor, bool useXmlContentType = false)
        {
            _options = options;
            _catalog = catalogAccessor;
            _useXmlContentType = useXmlContentType;
        }

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
                // One of 2 changes that make up Catalog2Provider
                if(_useXmlContentType)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "application/xml";
                }

                string query = context.Request.Params["X"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = $"{query}.xml";
            }
            else if (context.Request.Params["W"] != null)
            {
                // Two of 2 changes that make up Catalog2Provider
                if(_useXmlContentType)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "application/xml";
                }

                string query = context.Request.Params["W"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = $"{query}.wtml";
            }

            if (!string.IsNullOrEmpty(filename))
            {
                var catalogEntry = await _catalog.GetCatalogEntryAsync(filename);
                string newEtag = catalogEntry.LastModified.ToUniversalTime().ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", newEtag);
                    context.Response.AddHeader("Cache-Control", "no-cache");

                    using (var c = catalogEntry.Contents)
                    {
                        await c.CopyToAsync(context.Response.OutputStream);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
                else
                {
                    context.Response.Status = "304 Not Modified";
                }
            }
        }
    }
}

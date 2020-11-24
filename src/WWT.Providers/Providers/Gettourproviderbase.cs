#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public abstract class GetTourProviderBase : RequestProvider
    {
        protected abstract Task<Stream> GetStreamAsync(string id, CancellationToken token);

        public sealed override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string guid = context.Request.Params["GUID"];

            if (guid is null)
            {
                return;
            }

            using (var stream = await GetStreamAsync(guid, token))
            {
                if (stream is null)
                {
                    context.Response.Status = "404 Not Found";
                }
                else
                {
                    await stream.CopyToAsync(context.Response.OutputStream, token);
                }
            }
        }
    }
}

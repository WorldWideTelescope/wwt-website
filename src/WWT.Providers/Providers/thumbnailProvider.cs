using System.Threading;
using System.Threading.Tasks;
using WWTThumbnails;

namespace WWT.Providers
{
    public class thumbnailProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string name = context.Request.Params["name"];
            string type = context.Request.Params["class"];

            using (var s = WWTThumbnail.GetThumbnailStream(name, type))
            {
                s.CopyTo(context.Response.OutputStream);
                context.Response.Flush();
                context.Response.End();
            }

            return Task.CompletedTask;
        }
    }
}

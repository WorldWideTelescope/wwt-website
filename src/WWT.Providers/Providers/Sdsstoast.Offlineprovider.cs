using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/SDSSToast.offline.aspx")]
    public class SDSSToastOfflineProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Png;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write("Hello");
            return Task.CompletedTask;
        }
    }
}

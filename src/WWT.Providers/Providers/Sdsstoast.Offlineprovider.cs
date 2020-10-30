using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class SDSSToastOfflineProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write("Hello");
            return Task.CompletedTask;
        }
    }
}

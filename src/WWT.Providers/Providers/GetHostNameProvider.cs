using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class GetHostNameProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write(context.MachineName);
            return Task.CompletedTask;
        }
    }
}

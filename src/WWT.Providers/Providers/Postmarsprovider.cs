using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class PostMarsProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write("OK");
            return Task.CompletedTask;
        }
    }
}

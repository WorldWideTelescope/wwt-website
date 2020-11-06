using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/GetHostName.aspx")]
    public class GetHostNameProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write(context.MachineName);
            return Task.CompletedTask;
        }
    }
}

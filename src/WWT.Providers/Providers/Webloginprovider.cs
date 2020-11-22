using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/weblogin.aspx")]
    public class WebloginProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Expires = -1;
            await context.Response.WriteAsync("Key:Authorized", token);
        }
    }
}

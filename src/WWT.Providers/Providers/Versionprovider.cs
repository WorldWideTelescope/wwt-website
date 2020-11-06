using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/version.aspx")]
    public class VersionProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "version.txt")));
            return Task.CompletedTask;
        }
    }
}

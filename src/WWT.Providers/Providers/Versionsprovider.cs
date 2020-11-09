using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/versions.aspx")]
    public class VersionsProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            var wwt2dir = context.MapPath(Path.Combine("..", "wwt2"));

            await context.Response.WriteAsync("ClientVersion:", token);
            context.Response.WriteFile(Path.Combine(wwt2dir, "version.txt"));
            await context.Response.WriteAsync("\n", token);
            context.Response.WriteFile(Path.Combine(wwt2dir, "dataversion.txt"));
            await context.Response.WriteAsync("\nMessage:", token);
            context.Response.WriteFile(Path.Combine(wwt2dir, "message.txt"));
        }
    }
}

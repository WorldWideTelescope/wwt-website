using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class VersionsProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");

            var wwt2dir = context.MapPath(Path.Combine("..", "wwt2"));

            context.Response.Write("ClientVersion:");
            context.Response.WriteFile(Path.Combine(wwt2dir, "version.txt"));
            context.Response.Write("\n");
            context.Response.WriteFile(Path.Combine(wwt2dir, "dataversion.txt"));
            context.Response.Write("\nMessage:");
            context.Response.WriteFile(Path.Combine(wwt2dir, "message.txt"));

            return Task.CompletedTask;
        }
    }
}

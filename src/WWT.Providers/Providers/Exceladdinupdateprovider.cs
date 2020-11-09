using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/ExcelAddinUpdate.aspx")]
    public class ExcelAddinUpdateProvider : RequestProvider
    {
        public override bool IsCacheable => false;

        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            await context.Response.WriteAsync("ClientVersion:", token);
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinVersion.txt")));
            await context.Response.WriteAsync("\nUpdateUrl:", token);
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinUpdateUrl.txt")));
        }
    }
}

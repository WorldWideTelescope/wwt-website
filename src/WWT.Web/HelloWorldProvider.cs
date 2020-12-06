using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class HelloWorldProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => true;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string reply = $"WWT.Web app version {assemblyVersion}\n";
            await context.Response.WriteAsync(reply, token);
        }
    }
}

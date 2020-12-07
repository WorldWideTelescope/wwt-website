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
            var attr = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            string assemblyVersion = attr?.InformationalVersion ?? "0.0.0-unspecified";
            string reply = $"WWT.Web app version {assemblyVersion}\n";
            await context.Response.WriteAsync(reply, token);
        }
    }
}

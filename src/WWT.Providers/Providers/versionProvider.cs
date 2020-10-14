using System.IO;

namespace WWT.Providers
{
    public class VersionProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "version.txt")));
        }
    }
}

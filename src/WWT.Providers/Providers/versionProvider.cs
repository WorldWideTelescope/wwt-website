using System.Configuration;

namespace WWT.Providers
{
    public class versionProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"];
            context.Response.WriteFile(webDir + @"\wwt2\version.txt");
        }
    }
}

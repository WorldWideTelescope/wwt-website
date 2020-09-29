using System.Configuration;

namespace WWT.Providers
{
    public class ExcelAddinUpdateProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"];
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Write("ClientVersion:");
            context.Response.WriteFile(webDir + @"\wwt2\ExcelAddinVersion.txt");
            context.Response.Write("\nUpdateUrl:");
            context.Response.WriteFile(webDir + @"\wwt2\ExcelAddinUpdateUrl.txt");
        }
    }
}

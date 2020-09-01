using System.Configuration;

namespace WWT.Providers
{
    public class versionsProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string wwt2dir = ConfigurationManager.AppSettings["WWT2DIR"];
            context.Response.AddHeader("Cache-Control", "no-cache");

            context.Response.Write("ClientVersion:");
            context.Response.WriteFile(wwt2dir + @"\version.txt");
            context.Response.Write("\n");
            context.Response.WriteFile(wwt2dir + @"\dataversion.txt");
            context.Response.Write("\nMessage:");
            context.Response.WriteFile(wwt2dir + @"\message.txt");
        }
    }
}

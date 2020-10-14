using System.IO;

namespace WWT.Providers
{
    public class ExcelAddinUpdateProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Write("ClientVersion:");
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinVersion.txt")));
            context.Response.Write("\nUpdateUrl:");
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinUpdateUrl.txt")));
        }
    }
}

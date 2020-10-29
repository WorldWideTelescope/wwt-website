using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class loginProvider : LoginUser
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Expires = -1;
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("etag", "1-2-3-4-5");

            if (context.Request.Params["Equinox"] != null)
            {
                context.Response.WriteFile(context.MapPath("wwt2", "EqClientVersion.txt"));
                context.Response.Write("\n");
            }
            else
            {
                context.Response.Write("ClientVersion:");
                context.Response.WriteFile(context.MapPath("wwt2", "ClientVersion.txt"));
                context.Response.Write("\n");
                context.Response.WriteFile(context.MapPath("wwt2", "dataversion.txt"));
                context.Response.Write("\nMessage:");
                context.Response.WriteFile(context.MapPath("wwt2", "message.txt"));
                context.Response.Write("\nWarnVersion:");
                context.Response.WriteFile(context.MapPath("wwt2", "warnver.txt"));
                context.Response.Write("\nMinVersion:");
                context.Response.WriteFile(context.MapPath("wwt2", "minver.txt"));
                context.Response.Write("\nUpdateUrl:");
                context.Response.WriteFile(context.MapPath("wwt2", "updateurl.txt"));
            }
            context.Response.Flush();

            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]))
                {
                    String guid = context.Request.Params["user"];
                    String con = ConfigurationManager.AppSettings["LoggingConn"];
                    String ver = context.Request.Params["version"];
                    SqlConnection myConn = GetConnectionLogging(con);

                    PostLogin(myConn, guid, 1, ver);
                }
            }
            catch
            {
            }

            return Task.CompletedTask;
        }
    }
}

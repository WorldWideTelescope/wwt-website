using System;
using System.Configuration;
using System.Data.SqlClient;

namespace WWT.Providers
{
    public class loginProvider : LoginUser
    {
        public override void Run(WwtContext context)
        {
            string wwt2dir = ConfigurationManager.AppSettings["WWT2DIR"];
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Expires = -1;
            context.Response.CacheControl = "no-cache";
            context.Response.AddHeader("etag", "1-2-3-4-5");

            if (context.Request.Params["Equinox"] != null)
            {
                context.Response.WriteFile(wwt2dir + @"\EqClientVersion.txt");
                context.Response.Write("\n");
            }
            else
            {
                context.Response.Write("ClientVersion:");
                context.Response.WriteFile(wwt2dir + @"\ClientVersion.txt");
                context.Response.Write("\n");
                context.Response.WriteFile(wwt2dir + @"\dataversion.txt");
                context.Response.Write("\nMessage:");
                context.Response.WriteFile(wwt2dir + @"\message.txt");
                context.Response.Write("\nWarnVersion:");
                context.Response.WriteFile(wwt2dir + @"\warnver.txt");
                context.Response.Write("\nMinVersion:");
                context.Response.WriteFile(wwt2dir + @"\minver.txt");
                context.Response.Write("\nUpdateUrl:");
                context.Response.WriteFile(wwt2dir + @"\updateurl.txt");
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
        }
    }
}

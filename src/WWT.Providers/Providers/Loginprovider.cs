#nullable disable

using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/login.aspx")]
    public class LoginProvider : LoginUser
    {
        private readonly WwtOptions _options;

        public LoginProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Expires = -1;
            context.Response.AddHeader("etag", "1-2-3-4-5");

            if (context.Request.Params["Equinox"] != null)
            {
                context.Response.WriteFile(context.MapPath("wwt2", "EqClientVersion.txt"));
                await context.Response.WriteAsync("\n", token);
            }
            else
            {
                await context.Response.WriteAsync("ClientVersion:",token);
                context.Response.WriteFile(context.MapPath("wwt2", "ClientVersion.txt"));
                await context.Response.WriteAsync("\n", token);
                context.Response.WriteFile(context.MapPath("wwt2", "dataversion.txt"));
                await context.Response.WriteAsync("\nMessage:", token);
                context.Response.WriteFile(context.MapPath("wwt2", "message.txt"));
                await context.Response.WriteAsync("\nWarnVersion:", token);
                context.Response.WriteFile(context.MapPath("wwt2", "warnver.txt"));
                await context.Response.WriteAsync("\nMinVersion:", token);
                context.Response.WriteFile(context.MapPath("wwt2", "minver.txt"));
                await context.Response.WriteAsync("\nUpdateUrl:", token);
                context.Response.WriteFile(context.MapPath("wwt2", "updateurl.txt"));
            }
            context.Response.Flush();

            try
            {
                if (_options.LoginTracking)
                {
                    String guid = context.Request.Params["user"];
                    String con = _options.LoggingConn;
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

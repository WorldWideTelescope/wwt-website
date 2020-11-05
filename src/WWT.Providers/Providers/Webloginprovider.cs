using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/weblogin.aspx")]
    public class WebloginProvider : LoginWebUser
    {
        public WebloginProvider(WwtOptions options)
            : base(options)
        {
        }

        public override string ContentType => ContentTypes.Text;

        public override bool IsCacheable => false;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Expires = -1;

            string key = _options.Webkey;
            string testkey = context.Request.Params["webkey"];
            if (key == testkey)
            {
                context.Response.Write("Key:Authorized");
                if (_options.LoginTracking)
                {
                    byte platform = 2;
                    if (context.Request.Params["Version"] != null)
                    {
                        if (context.Request.Params["Version"] == "BING")
                        {
                            platform = 4;
                        }



                        if (context.Request.Params["Version"] == "HTML5")
                        {
                            platform = 8;
                        }

                    }

                    if (context.Request.Params["platform"] != null)
                    {
                        if (context.Request.Params["platform"] == "MAC")
                        {
                            platform += 1;
                        }
                    }

                    PostFeedback(context.Request.Params["user"], platform);
                }
            }
            else
            {
                context.Response.Write("Key:Failed");
            }

            return Task.CompletedTask;
        }
    }
}

using System;
using System.Configuration;

namespace WWT.Providers
{
    public class webloginProvider : LoginWebUser
    {
        public override void Run(IWwtContext context)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Expires = -1;
            context.Response.CacheControl = "no-cache";

            string key = ConfigurationManager.AppSettings["webkey"];
            string testkey = context.Request.Params["webkey"];
            if (key == testkey)
            {
                context.Response.Write("Key:Authorized");
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]))
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
        }
    }
}

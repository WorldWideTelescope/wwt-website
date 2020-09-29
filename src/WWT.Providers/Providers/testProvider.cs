using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace WWT.Providers
{
    public class testProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string primary = ConfigurationManager.AppSettings["PrimaryFileserver"].ToLower();
            context.Response.Write("primary: " + primary + "<BR />");
            string backup = ConfigurationManager.AppSettings["BackupFileserver"].ToLower();
            context.Response.Write("Secondary: " + backup + "<BR />");
            string current = (string)HttpContext.Current.Cache.Get("CurrentFileServer");
            context.Response.Write("Current: " + current + "<BR />");

            DateTime lastCheck = DateTime.Now.AddDays(-1);

            if (!string.IsNullOrEmpty(current) && HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime") != null)
            {
                lastCheck = (DateTime)HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime");
            }

            TimeSpan ts = DateTime.Now - lastCheck;

            if (ts.TotalMinutes > 1)
            {
                HttpContext.Current.Cache.Remove("LastFileserverUpdateDateTime");
                HttpContext.Current.Cache.Add("LastFileserverUpdateDateTime", System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);


                if (string.IsNullOrEmpty(current) || !Directory.Exists(@"\\" + current + @"\wwttours"))
                {
                    bool primaryUp = false;

                    try
                    {
                        primaryUp = Directory.Exists(@"\\" + primary + @"\wwttours");

                        context.Response.Write("  primary: " + primary + @"\wwttours      " + "<BR />");
                        context.Response.Write("Is primary up: " + primaryUp + "<BR />");
                    }
                    catch
                    {
                    }

                    if (primaryUp)
                    {
                        current = primary;
                        HttpContext.Current.Cache.Remove("CurrentFileServer");
                        HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                    }
                    else
                    {
                        current = backup;
                        HttpContext.Current.Cache.Remove("CurrentFileServer");
                        HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                }
            }

            String baseName = ConfigurationManager.AppSettings["WWTToursTourFileUNC"].ToLower();

            context.Response.Write(baseName.Replace(primary, current));
        }
    }
}

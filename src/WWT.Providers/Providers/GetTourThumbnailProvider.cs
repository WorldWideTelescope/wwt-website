using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetTourThumbnailProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string guid;
            if (context.Request.Params["GUID"] != null)
            {
                guid = context.Request.Params["GUID"];
            }
            else
            {
                context.Response.End();
                return Task.CompletedTask;
            }
            string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];
            string localDir = tourcache;
            string filename = ConfigurationManager.AppSettings["WWTToursTourFileUNC"] + String.Format(@"\{0}_TourThumb.bin", guid);
            string localfilename = localDir + String.Format(@"\{0}_TourThumb.bin", guid);

            if (!File.Exists(localfilename))
            {
                try
                {


                    if (File.Exists(filename))
                    {
                        if (!Directory.Exists(localDir))
                        {
                            Directory.CreateDirectory(localDir);
                        }
                        File.Copy(filename, localfilename);
                    }

                }
                catch
                {
                }
            }

            if (File.Exists(localfilename))
            {
                try
                {
                    context.Response.ContentType = "image/png";
                    context.Response.WriteFile(localfilename);
                    return Task.CompletedTask;
                }
                catch
                {
                }
            }
            else
            {
                context.Response.Status = "404 Not Found";
            }

            return Task.CompletedTask;
        }
    }
}

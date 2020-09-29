using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetAuthorThumbnailProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string guid;
            if (context.Request.Params["GUID"] != null)
            {
                guid = context.Request.Params["GUID"];
            }
            else
            {
                context.Response.End();
                return;
            }
            string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];
            string localDir = tourcache;
            string filename = WWTUtil.GetCurrentConfigShare("WWTToursTourFileUNC", true) + String.Format(@"\{0}_AuthorThumb.bin", guid);
            string localfilename = localDir + String.Format(@"\{0}_AuthorThumb.bin", guid);

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
                    return;
                }
                catch
                {
                }
            }
            else
            {
                context.Response.Status = "404 Not Found";
            }
        }
    }
}

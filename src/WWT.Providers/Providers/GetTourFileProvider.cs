using System;
using System.IO;
using System.Net;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetTourFileProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string returnString = "Error: No URL Specified";
            string url = "";

            string path = context.Server.MapPath(@"TourCache");

            try
            {
                if (context.Request.Params["targeturl"] != null && context.Request.Params["filename"] != null)
                {
                    url = context.Request.Params["targeturl"];

                    string targetfile = context.Request.Params["filename"];
                    string filename = path + "\\" + Math.Abs(url.GetHashCode()) + ".wtt";

                    if (!File.Exists(filename))
                    {
                        if (url.ToLower().StartsWith("http"))
                        {
                            using (WebClient wc = new WebClient())
                            {
                                byte[] data = wc.DownloadData(url);

                                //context.Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
                                int length = data.Length;

                                File.WriteAllBytes(filename, data);
                                //context.Response.OutputStream.Write(data, 0, length);
                            }
                        }
                    }


                    FileCabinet.Extract(filename, targetfile, context.Response);
                }
            }
            catch (System.Exception e)
            {
                returnString = e.Message;
                context.Response.Write(returnString);
            }
        }
    }
}

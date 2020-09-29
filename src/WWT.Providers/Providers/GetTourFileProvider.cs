using System;
using System.IO;
using System.Net;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetTourFileProvider : RequestProvider
    {
        private readonly IFileNameHasher _hasher;

        public GetTourFileProvider(IFileNameHasher hasher)
        {
            _hasher = hasher;
        }

        public override void Run(IWwtContext context)
        {
            string path = context.Server.MapPath(@"TourCache");

            try
            {
                if (context.Request.Params["targeturl"] != null && context.Request.Params["filename"] != null)
                {
                    var url = context.Request.Params["targeturl"];
                    string targetfile = context.Request.Params["filename"];
                    string filename = Path.Combine(path, $"{_hasher.HashName(url)}.wtt");

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
            catch (Exception e)
            {
                context.Response.Write(e.Message);
            }
        }
    }
}

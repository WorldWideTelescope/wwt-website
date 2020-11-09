using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/GetTourFile.aspx")]
    [RequestEndpoint("/GetTourFile.aspx")]
    [RequestEndpoint("/GetTourFile2.aspx")]
    public class GetTourFileProvider : RequestProvider
    {
        private readonly IFileNameHasher _hasher;

        public GetTourFileProvider(IFileNameHasher hasher)
        {
            _hasher = hasher;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string path = context.MapPath(@"TourCache");

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

                    var (contentType, result) = FileCabinet.Extract(filename, targetfile);

                    if (result != null)
                    {
                        context.Response.ContentType = contentType;
                        context.Response.OutputStream.Write(result, 0, result.Length);
                    }
                }
            }
            catch (Exception e)
            {
                await context.Response.WriteAsync(e.Message, token);
            }
        }
    }
}

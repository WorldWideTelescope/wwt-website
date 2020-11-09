using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/WebServiceProxy.aspx")]
    public class WebServiceProxyProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string returnString = "Erorr: No URL Specified";
            string url = "";
            if (context.Request.Params["targeturl"] != null)
            {
                url = context.Request.Params["targeturl"];

                try
                {
                    if (url.ToLower().StartsWith("http") && !url.Contains("127.0.0.1") && !url.ToLower().Contains("localhost"))
                    {
                        Uri target = new Uri(url);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        using (WebClient wc = new WebClient())
                        {
                            byte[] data = wc.DownloadData(url);

                            context.Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
                            int length = data.Length;
                            context.Response.OutputStream.Write(data, 0, length);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    returnString = e.Message;
                    await context.Response.WriteAsync(returnString, token);
                }
            }
        }
    }
}

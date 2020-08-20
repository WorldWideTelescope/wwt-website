<%@ WebHandler Language="C#"  Class="AsyncProxy" %>
using System;
using System.Web;
using System.Threading.Tasks;
using System.Net;

public class AsyncProxy : HttpTaskAsyncHandler
{
  
  public override async Task ProcessRequestAsync(HttpContext context)
  {
    string returnString = "Error: No URL Specified";
    string url = "";

    var Request = context.Request;
    if (Request.Params["targeturl"] != null)
    {
      url = Request.Params["targeturl"];

      try
      {
        if (url.ToLower().StartsWith("http") && !url.Contains("127.0.0.1") && !url.ToLower().Contains("localhost"))
        {
          ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
          using (WebClient wc = new WebClient())
          {
            byte[] data =await wc.DownloadDataTaskAsync(new Uri(url));
            context.Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
            int length = data.Length;
            context.Response.OutputStream.Write(data, 0, length);           

          }
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

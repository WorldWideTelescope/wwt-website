<%@ Page Language="C#" Async="true" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
    string returnString = "Error: No URL Specified";
    string url = "";

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
            byte[] data = null;
            wc.DownloadDataAsync(new Uri(url));
            wc.DownloadDataCompleted +=
            delegate(object sender, DownloadDataCompletedEventArgs e)
            {
              data = e.Result;
              Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
              int length = data.Length;
              Response.OutputStream.Write(data, 0, length);
            };

          }
        }
      }
      catch (System.Exception e)
      {
        returnString = e.Message;
        Response.Write(returnString);
      }
    }

%>
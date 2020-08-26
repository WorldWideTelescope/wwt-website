<%@ Page Language="C#" ContentType="image/jpeg" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="WWTThumbnails" %>
<%
    string name = Request.Params["name"];
    string type = Request.Params["class"];

    Stream s = WWTThumbnail.GetThumbnailStream(name);
    if (s == null && type != null)
    {
        s = WWTThumbnail.GetThumbnailStream(type);
    }

    if (s == null)
    {
        string dataDir = ConfigurationManager.AppSettings["DataDir"];

        var jpeg = Path.Combine(dataDir, "thumbnails", name + ".jpg");
        if (File.Exists(jpeg))
        {
            Response.WriteFile(jpeg);
            Response.End();
        }

        s = File.OpenRead(Path.Combine(dataDir, "thumbnails", "Star.jpg"));
    }

    int length = (int)s.Length;
    byte[] data = new byte[length];
    s.Read(data, 0, length);
    Response.OutputStream.Write(data, 0, length);
    Response.Flush();
    Response.End();
    s.Dispose();

	%>
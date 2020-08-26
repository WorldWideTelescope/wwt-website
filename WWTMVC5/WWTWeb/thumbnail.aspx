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

    using (var s = WWTThumbnail.GetThumbnailStream(name, type))
    {
        s.CopyTo(Response.OutputStream);
        Response.Flush();
        Response.End();
    }
%>
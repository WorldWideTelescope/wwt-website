<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="hello.aspx.cs" %>
<%@ Import Namespace="WWTMVC5.WWTWeb" %>
<%@ Import Namespace="WWTWebservices" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Hello!</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>WWT Running on Server <% = System.Environment.MachineName %></h1>
    <h1>WWT File Server <% = WWTUtil.GetCurrentConfigShare("DSSTOASTPNG", true) %></h1>
    <h1>URL was Running on <% =  Server.HtmlEncode(Request.RawUrl) %></h1>
   <h1>URL was Running on <% =  Request.Headers["host"] %></h1>
    </div>
    </form>
</body>
</html>

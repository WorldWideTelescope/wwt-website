<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XML2WTT.aspx.cs" Inherits="WWTMVC5.WWTWeb.WWTWeb_XML2WTT" %>

<%	
    //string etag = Request.Headers["If-None-Match"];
    string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];

    Response.ClearHeaders();
    Response.Clear();
    Response.ContentType="application/x-wtt";

    Response.WriteFile(MakeTourFromXML(Request.InputStream, tourcache + "\\temp\\"));
    
    //Response.OutputStream
    
%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="app_code\GetTours.aspx.cs" Inherits="GetTours" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml.Serialization" %>

<%
    Response.ClearHeaders();
    Response.Clear();
    Response.ContentType="text/xml";

    string toursXML = null;
    UpdateCache();
    toursXML = (string)HttpContext.Current.Cache["WWTXMLTours"];

    if (toursXML != null)
    {
       Response.Write(toursXML);
    }
    Response.End();
    
%>

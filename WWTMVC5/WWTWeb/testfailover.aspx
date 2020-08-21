<%@ Page Language="C#" ContentType="text/html" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="WWTWebservices" %>
<%
       
                Response.Write(WWTUtil.GetCurrentConfigShare("DSSTOASTPNG", true));


	
	%>
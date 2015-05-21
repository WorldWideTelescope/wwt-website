<%@ Page Language="C#" ContentType="text/plain" %>
<%
	string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"];
    Response.AddHeader("Cache-Control", "no-cache");
    Response.Write("ClientVersion:");
    Response.WriteFile(webDir + @"\wwt2\ExcelAddinVersion.txt");
    Response.Write("\nUpdateUrl:");
    Response.WriteFile(webDir + @"\wwt2\ExcelAddinUpdateUrl.txt");
%>
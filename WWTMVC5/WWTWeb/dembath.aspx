<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.IO" %>

<%
    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    int level = Convert.ToInt32(values[0]);
    int tileX = Convert.ToInt32(values[1]);
    int tileY = Convert.ToInt32(values[2]);
	
    string filename = String.Format(  "D:\\DEM\\bath\\{0}\\{1}\\L{0}X{1}Y{2}.dem", level, tileX, tileY);

    if (!File.Exists(filename))
    {
   	    Response.StatusCode = 404;
            return;
    }
    
    Response.WriteFile(filename);

%>
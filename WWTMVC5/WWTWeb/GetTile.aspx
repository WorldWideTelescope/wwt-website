<%@ Page Language="C#" ContentType="image/png" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="WWTWebservices" %>
<%

    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    int level = Convert.ToInt32(values[0]);
    int tileX = Convert.ToInt32(values[1]);
    int tileY = Convert.ToInt32(values[2]);
    string dataset = values[3];
    string id = dataset;
    string type = ".png";
	
    string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);

    string filename = String.Format( DSSTileCache  + "\\imagesTiler\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, id);
    
    if (!File.Exists(filename))
    {
        Response.StatusCode = 404;
        return;
    }
    
    Response.WriteFile(filename);
    
%>
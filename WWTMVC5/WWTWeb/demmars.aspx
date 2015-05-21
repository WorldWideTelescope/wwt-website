<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="WWTMVC5.WWTWeb" %>
<%@ Import Namespace="WWTWebservices" %>
<%
    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    int level = Convert.ToInt32(values[0]);
    int tileX = Convert.ToInt32(values[1]);
    int tileY = Convert.ToInt32(values[2]);
	
    string file = "mars";
    //string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
    string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);

    DSSTileCache = @"\\wwt-sql01\dsstilecache";

    string filename = String.Format(DSSTileCache  + "\\wwtcache\\mars\\dem\\{0}\\{2}\\{1}_{2}.dem", level, tileX, tileY);

    //string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\dem\\{0}\\{2}", level, tileX, tileY);



    if (!File.Exists(filename))
    {
       // try
       // {
       //     if (!Directory.Exists(filename))
       //     {
       //         Directory.CreateDirectory(path);
       //     }
            
       //     WebClient webclient = new WebClient();

       //     string url = string.Format("http://wwt.nasa.gov/wwt/p/mars_toast_dem_32f/{0}/{1}/{2}.toast_dem_v1",  level, tileX, tileY);

       //     webclient.DownloadFile(url, filename);
      //  }
      //  catch
       // {
            Response.StatusCode = 404;
            return;
       // }
    }
    
    Response.WriteFile(filename);
    
%>
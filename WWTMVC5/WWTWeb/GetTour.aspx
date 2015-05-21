<%@ Page Language="C#" ContentType="application/x-wtt" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="WWTWebservices" %>
<%
    string guid;
    if (Request.Params["GUID"] != null)
    {
        guid = Request.Params["GUID"];
    }
    else
    {
        Response.End();
        return;
    }
    string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];
    string localDir = tourcache;
    string filename = WWTUtil.GetCurrentConfigShare("WWTToursTourFileUNC", true) +String.Format(@"\{0}.bin", guid);
 	string localfilename = localDir  +String.Format(@"\{0}.bin", guid);

    if (!File.Exists(localfilename))
    {
        try
        {
            if(File.Exists(filename))
	        {
	    	    if (!Directory.Exists(localDir))
		        {
		            Directory.CreateDirectory(localDir);
		        }		
		        File.Copy(filename,localfilename);
	        }
        }
        catch
        {
        }
    }
	
    if (File.Exists(localfilename))
    {
        try
        {
            Response.ContentType = "application/x-wtt";
            Response.WriteFile(localfilename);
            return;
        }
        catch
        {
        }
    }
	else
	{
	    Response.Status = "404 Not Found";
	}
	
%>
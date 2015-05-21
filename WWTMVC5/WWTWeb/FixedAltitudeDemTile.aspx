<%@ Page Language="C#" ContentType="application/octet-stream" %>
<%@ Import Namespace="System.IO" %>
<%
    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    //int level = Convert.ToInt32(values[0]);
    //int tileX = Convert.ToInt32(values[1]);
    //int tileY = Convert.ToInt32(values[2]);
	string alt = Request.Params["alt"];
	string proj = Request.Params["proj"];
	float altitude = float.Parse(alt);
    int demSize = 33 * 33;

	if (proj.ToLower().StartsWith("t"))
	{
		demSize = 17 * 17;
	}

        BinaryWriter bw = new BinaryWriter(Response.OutputStream);
	
	for(int i = 0; i < demSize; i++)
	{
		bw.Write(altitude);
	}
    
 	bw = null;
    Response.OutputStream.Flush();

    Response.End();
	
%>
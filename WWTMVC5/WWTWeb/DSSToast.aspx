<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="WWTMVC5.WWTWeb" %>
<%@ Import Namespace="WWTWebservices" %>
<%
    	string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
    	string dsstoastpng = WWTUtil.GetCurrentConfigShare("DSSTOASTPNG", true);
   
    
    
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);

        int octsetlevel = level;
        string filename;

        if (level > 12)
        {
            Response.Write("No image");
            Response.Close();
            return;
        }

        if (level < 8)
        {
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\dsstoast.plate", level, tileX, tileY);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }	
	else
	{
	    int L = level;
	    int X = tileX;
	    int Y = tileY;
	    string mime = "png";	
	    int powLev5Diff = (int) Math.Pow(2, L - 5);
            int X32 = X / powLev5Diff;
            int Y32 = Y / powLev5Diff;
            filename = string.Format(dsstoastpng + @"\DSS{0}L5to12_x{1}_y{2}.plate", mime, X32, Y32);
           
            int L5 = L - 5;
            int X5 = X % powLev5Diff; 
            int Y5 = Y % powLev5Diff;
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(filename, L5, X5, Y5);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
	
	}
   
    // This file has returns which cause this warning to show in the generated files.
    // This should be refactored, but that will be a bigger change.
    #pragma warning disable 0162
    
	%>
<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="PlateFile2" %>
<%
   
    
    
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);

            try
            {
                Response.ContentType = "image/png";
                CloudBlockBlob blob = new CloudBlockBlob(new Uri(@"https://marsstage.blob.core.windows.net/marsbasemap/marsbasemap.plate"));
  
            	Stream s = PlateFile2.PlateFile2.GetFileStream(blob.OpenRead(), -1, level, tileX, tileY);

                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                Response.OutputStream.Write(data, 0, length);
                Response.Flush();
                Response.End();
                return;
            }
            catch
            {
		Response.Clear();
		Response.ContentType = "text/plain";
                Response.Write("No image");
                Response.End();
                return;
            }


    
	
	%>
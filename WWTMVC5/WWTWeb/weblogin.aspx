<%@ Page Language="C#" ContentType="text/plain"  AutoEventWireup="true" CodeFile="weblogin.aspx.cs" Inherits="WWTMVC5.WWTWeb.LoginWebUser" %>
<%
		Response.AddHeader("Cache-Control", "no-cache");
                Response.Expires = -1;
                Response.CacheControl = "no-cache";

	string key = ConfigurationManager.AppSettings["webkey"];
	string testkey = Request.Params["webkey"];
	if (key == testkey)
	{
		Response.Write("Key:Authorized");
		if (Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]))
		{	
			byte platform = 2;
            if (Request.Params["Version"] != null)
            {
                if (Request.Params["Version"] == "BING")
			{
				platform = 4;
			}
                
            

                	if (Request.Params["Version"] == "HTML5")
			{
				platform = 8;
			}
                
		}
            
			if (Request.Params["platform"] != null)
			{
				if (Request.Params["platform"]== "MAC")
				{
					platform +=1;
				}
			}

			PostFeedback( Request.Params["user"], platform );
		}
	}
	else
	{
		Response.Write("Key:Failed");
	}


%>
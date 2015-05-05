using WWTMVC5.WebServices;
using System.Web.Configuration;
using Microsoft.Live;

namespace WWTMVC5.Models
{
	public class BaseModel
	{
		private readonly string _resLoc = WebConfigurationManager.AppSettings["ResourcesLocation"];
		private readonly bool _staging = WebConfigurationManager.AppSettings["Staging"] == "true";

       
		public string ResLoc
		{
			get { return _resLoc; }
		}

		public string CssDir
		{
			get { return _resLoc + "/Content/CSS"; }
		}

		public string JsDir
		{
			get { return _resLoc + "/Scripts"; }
		}

		public string ImgDir
		{
			get { return _resLoc + "/Content/Images"; }
		}
		public string ContentDir
		{
			get { return _resLoc + "/Content"; }
		}

		public bool Staging
		{
			get { return _staging; }
		}

		public string ToursDir
		{
			get { return _staging ? "http://thewebkid.com/WebControlTours" : _resLoc + "/Content/WebControlTours"; }
		}


		public string ResVer { get { return WebConfigurationManager.AppSettings["ResourcesVersion"]; } }

		public string DownloadUrl { get { return WebConfigurationManager.AppSettings["WWTDownloadUrl"]; } }

		public string LegacyUrl { get { return WebConfigurationManager.AppSettings["WWTLegacyDownloadUrl"]; } }

	    public ProfileDetails User
	    {
	        get
	        {
	            ProfileDetails profileDetails = null;
	            LiveLoginResult result = SessionWrapper.Get<LiveLoginResult>("LiveConnectResult");
	            if (result != null && result.Status == LiveConnectSessionStatus.Connected)
	            {
	                profileDetails = SessionWrapper.Get<ProfileDetails>("ProfileDetails");

	            }
	            return profileDetails;
	        }
	    }
	}
}
using WWTMVC5.WebServices;
using System.Web.Configuration;
using Microsoft.Live;

namespace WWTMVC5.Models
{
	public class BaseModel
	{
	    private readonly string _contentDir;
	    private readonly string _imgDir;
	    private readonly string _jsDir;
	    private readonly string _cssDir;
	    private readonly string _resLoc;
	    private readonly bool _staging;
	    private readonly string _toursDir;
	    private readonly string _resVer;
	    private readonly string _downloadUrl;
	    private readonly string _legacyUrl;
	    private bool _isOpenWwtKiosk = false;


	    public BaseModel()
	    {
	        _resLoc = ConfigReader<string>.GetSetting("ResourcesLocation");//CDN
	        _staging = ConfigReader<bool>.GetSetting("Staging");
            _cssDir = ResLoc + "/Content/CSS"; 
            _jsDir = ResLoc + "/Scripts";
            _contentDir= "//wwtweb.blob.core.windows.net";//azure blob storage
	        _imgDir = ContentDir + "/images";
            _toursDir = ContentDir + "/WebControlTours";
            _resVer = ConfigReader<string>.GetSetting("ResourcesVersion");
            _downloadUrl = ConfigReader<string>.GetSetting("WWTDownloadUrl");
            _legacyUrl = ConfigReader<string>.GetSetting("WWTLegacyDownloadUrl");
	    }

	    public string ResLoc
	    {
	        get { return _resLoc; }
	    }

	    public string CssDir
	    {
	        get { return _cssDir; }
	    }

	    public string JsDir
	    {
	        get { return _jsDir; }
	    }

	    public string ContentDir
	    {
	        get { return _contentDir; }
	    }

	    public string ImgDir
	    {
	        get { return _imgDir; }
	    }

	    public bool Staging
	    {
	        get { return _staging; }
	    }

	    public string ToursDir
	    {
	        get { return _toursDir; }
	    }

	    public string ResVer
	    {
	        get { return _resVer; }
	    }

	    public string DownloadUrl
	    {
	        get { return _downloadUrl; }
	    }

	    public string LegacyUrl
	    {
	        get { return _legacyUrl; }
	    }

	    public bool IsOpenWwtKiosk
	    {
	        get { return _isOpenWwtKiosk; }
	        set { _isOpenWwtKiosk = value; }
	    }

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
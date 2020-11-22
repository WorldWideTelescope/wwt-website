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
        private readonly string _toursDir;
        private readonly string _resVer;
        private readonly string _downloadUrl;
        private readonly string _legacyUrl;
        private ProfileDetails _profile;

        public BaseModel()
        {
            _cssDir = "/Content/CSS";
            _jsDir = "/Scripts";
            _contentDir = "//wwtweb.blob.core.windows.net";
            _imgDir = ContentDir + "/images";
            _toursDir = ContentDir + "/WebControlTours";
            _resVer = ConfigReader<string>.GetSetting("ResourcesVersion");
            _downloadUrl = ConfigReader<string>.GetSetting("WWTDownloadUrl");
            _legacyUrl = ConfigReader<string>.GetSetting("WWTLegacyDownloadUrl");
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

        public ProfileDetails User
        {
            get
            {
                if (_profile != null)
                {
                    return _profile;
                }
                var profileDetails = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
                return profileDetails;
            }

            set { _profile = value; }
        }
    }
}

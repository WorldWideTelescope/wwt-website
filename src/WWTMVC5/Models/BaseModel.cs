using WWTMVC5.WebServices;
using System.Web.Configuration;
using Microsoft.Live;

namespace WWTMVC5.Models
{
    public class BaseModel
    {
        private readonly string _contentDir;
        private readonly string _jsDir;
        private readonly string _cssDir;
        private readonly string _resVer;
        private ProfileDetails _profile;

        public BaseModel()
        {
            _cssDir = "/Content/CSS";
            _jsDir = "/Scripts";
            _contentDir = "//wwtweb.blob.core.windows.net";
            _resVer = ConfigReader<string>.GetSetting("ResourcesVersion");
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

        public string ResVer
        {
            get { return _resVer; }
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

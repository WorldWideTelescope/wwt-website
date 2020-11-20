using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Live;
using Newtonsoft.Json;

namespace WWTMVC5.WebServices
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LiveIdAuth
    {
        private string _clientId;
        private string _clientSecret;
        private LiveAuthClient _liveAuthClient;
        private LiveConnectSession _session = null;

        public LiveIdAuth()
        {
            _clientId = ConfigReader<string>.GetSetting("LiveClientId");
            _clientSecret = ConfigReader<string>.GetSetting("LiveClientSecret");
            _liveAuthClient = new LiveAuthClient(ConfigReader<string>.GetSetting("LiveClientId"), ConfigReader<string>.GetSetting("LiveClientSecret"), null);
        }

        [WebGet]
        [OperationContract]
        public async Task<LiveLoginResult> Authenticate()
        {
            LiveLoginResult result = null;
            try
            {
                var redir = GetRedirectUrl();
                result = await _liveAuthClient.InitializeWebSessionAsync(new HttpContextWrapper(HttpContext.Current), redir, new[] { "wl.emails", "wl.signin" });

                _session = result.Session;
            }
            catch (LiveAuthException) { }
            return result;
        }

        private string GetRedirectUrl()
        {
            var redir = HttpContext.Current.Request.UrlReferrer != null ? 
                HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Split('?')[0] :
                "http://" + HttpContext.Current.Request.Headers.Get("host");
            if (redir.EndsWith("/"))
            {
                redir = redir.TrimEnd(new char[] { '/' });
            }
            return redir;
        }

        public async Task<string> GetTokens(string authCode)
        {
            // This call is purely internal, so use the "desktop" redirect_uri. Our WWT ones
            // are currently (2020 Nov) disabled, possibly because they are HTTP not HTTPS.
            var redir = "https://login.live.com/oauth20_desktop.srf";
            var tokenUri = new Uri(string.Format("https://login.live.com/oauth20_token.srf?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code",
                _clientId, HttpUtility.UrlEncode(redir), _clientSecret, authCode));

            

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(tokenUri);
            var responseString = await response.Content.ReadAsStringAsync();
            
            var tokens = new { access_token = "", refresh_token = "" };
            var json = JsonConvert.DeserializeAnonymousType(responseString, tokens);
            HttpCookie authCookie = new HttpCookie("refresh_token", json.refresh_token) { Expires = DateTime.MaxValue };
            HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpCookie accessCookie = new HttpCookie("access_token", json.access_token) { Expires = DateTime.MaxValue };
            HttpContext.Current.Response.Cookies.Add(accessCookie);
            return responseString;
        }

        private static byte[] CreatePostData<T>(T value)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return stream.ToArray();
            }
        }

        public async Task<string> RefreshTokens()
        {
            var token = HttpContext.Current.Request.Cookies["refresh_token"] != null ?HttpContext.Current.Request.Cookies["refresh_token"].Value : null;
            if (token == null)
            {
                return string.Empty;
            }
            // This call is purely internal, so use the "desktop" redirect_uri. Our WWT ones
            // are currently (2020 Nov) disabled, possibly because they are HTTP not HTTPS.
            var redir = "https://login.live.com/oauth20_desktop.srf";
            
            var tokenUri = string.Format("https://login.live.com/oauth20_token.srf?client_id={0}&redirect_uri={1}&client_secret={2}&refresh_token={3}&grant_type=refresh_token",
                _clientId, HttpUtility.UrlEncode(redir), _clientSecret, token);

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(tokenUri);
            var responseString = await response.Content.ReadAsStringAsync();


            var tokens = new { access_token = "", refresh_token = "" };
            var json = JsonConvert.DeserializeAnonymousType(responseString, tokens);
            HttpCookie authCookie = new HttpCookie("refresh_token", json.refresh_token) { Expires = DateTime.MaxValue };
            HttpContext.Current.Response.Cookies.Add(authCookie);
            HttpCookie accessCookie = new HttpCookie("access_token", json.access_token) { Expires = DateTime.MaxValue };
            HttpContext.Current.Response.Cookies.Add(accessCookie);
            return responseString;
        }
        
        

        [WebGet]
        [OperationContract]
        public async Task<string> GetUserId(string accessToken)
        {
            var meUri = new Uri(string.Format("https://apis.live.net/v5.0/me/?access_token={0}", accessToken));

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(meUri);
            var responseString = await response.Content.ReadAsStringAsync();
            var meObj = new { Id = "" };

            meObj = JsonConvert.DeserializeAnonymousType(responseString, meObj);
            return meObj.Id;
            // This method will not work - returns a 32 character user id
            // var userId = _liveAuthClient.GetUserId(authToken);//
            // return userId;
        }

        [WebGet]
        [OperationContract]
        public async Task<dynamic> GetMeInfo(string accessToken)
        {
            var meUri = new Uri(string.Format("https://apis.live.net/v5.0/me/?access_token={0}", accessToken));
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(meUri);
            var responseString = await response.Content.ReadAsStringAsync();
            return (dynamic)JsonConvert.DeserializeObject(responseString);
        }

        public string GetLogoutUrl(string returnUrl)
        {
            return _liveAuthClient.GetLogoutUrl(returnUrl);
        }

    }
}

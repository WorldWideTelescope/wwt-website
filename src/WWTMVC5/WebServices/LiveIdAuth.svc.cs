using System;
using Unity;
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
using Microsoft.Extensions.Logging;

namespace WWTMVC5.WebServices
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LiveIdAuth
    {
        private readonly ILogger<LiveIdAuth> _logger;

        private string _clientId;
        private string _clientSecret;
        private LiveAuthClient _liveAuthClient;
        private LiveConnectSession _session = null;

        public LiveIdAuth()
        {
            _clientId = ConfigReader<string>.GetSetting("LiveClientId");
            _clientSecret = ConfigReader<string>.GetSetting("LiveClientSecret");
            _liveAuthClient = new LiveAuthClient(ConfigReader<string>.GetSetting("LiveClientId"), ConfigReader<string>.GetSetting("LiveClientSecret"), null);
            _logger = UnityConfig.Container.Resolve<ILogger<LiveIdAuth>>();
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
            // There used to be more complex logic here, but for the production
            // OAuth app, only two redirection URLs are (known to be) allowed:
            // http://worldwidetelescope.org/webclient, and
            // http://www.worldwidetelescope.org/webclient , and you need to use
            // the same URL consistently through the auth process to keep Live
            // happy. (Unfortunately, we can't update these URLs and https:
            // variants aren't allowed.)
            return ConfigReader<string>.GetSetting("LiveClientRedirectUrl");
        }

        public async Task<string> GetTokens(string authCode)
        {
            _logger.LogInformation("OAuth: GetTokens()");

            var redir = GetRedirectUrl();
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

            var redir = GetRedirectUrl();

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

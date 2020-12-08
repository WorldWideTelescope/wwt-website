using System;
using System.Collections.Generic;
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

using Microsoft.Extensions.Logging;
using Microsoft.Live;
using Newtonsoft.Json;
using Unity;

namespace WWTMVC5.WebServices
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LiveIdAuth
    {
        private readonly ILogger<LiveIdAuth> _logger;

        private string _clientId;
        private string _clientSecret;
        private Dictionary<string, string> _redirectUris;
        private LiveAuthClient _liveAuthClient;
        private LiveConnectSession _session = null;

        public LiveIdAuth()
        {
            _clientId = ConfigReader<string>.GetSetting("LiveClientId");
            _clientSecret = ConfigReader<string>.GetSetting("LiveClientSecret");
            _liveAuthClient = new LiveAuthClient(_clientId, _clientSecret, null);

            // This file used to have some code to construct redirection URLs on
            // the fly, but that doesn't work well because the URLs have to
            // match values that are exactly specified in the OAuth app, and
            // this server may be running behind a gateway such that its
            // hostname is unrelated to what we want to expose. On the other hand,
            // the way that Azure deployment slots work, our staging and production
            // apps have to share identical settings if we want to use its "swap"
            // functionality -- so we can't just hardcode a single redirection URL.
            // So we have a variable that maps from HTTP host to redirection URL,
            // in the form "host1=http://url1/,host2=http://url1".
            var redirText = ConfigReader<string>.GetSetting("LiveClientRedirectUrlMap");
            _redirectUris = new Dictionary<string, string>();

            foreach (var item in redirText.Split(',')) {
                var pieces = item.Split(new char[] { '=' }, 2);
                var host = pieces[0].ToLower();
                var redirectUri = pieces[1];
                _redirectUris[host] = redirectUri;
            }

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

        public string GetRedirectUrl()
        {
            var host = HttpContext.Current.Request.Headers.Get("Host").ToLower();

            if (_redirectUris.ContainsKey(host)) {
                return _redirectUris[host];
            }

            return "https://" + host + "/";
        }

        public async Task<string> GetTokens(string authCode)
        {
            _logger.LogInformation("OAuth: GetTokens()");

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("client_id", _clientId));
            formData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));
            formData.Add(new KeyValuePair<string, string>("redirect_uri", GetRedirectUrl()));
            formData.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            formData.Add(new KeyValuePair<string, string>("code", authCode));

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://login.live.com/oauth20_token.srf", new FormUrlEncodedContent(formData));
            var responseString = await response.Content.ReadAsStringAsync();

            var tokens = new { access_token = "", refresh_token = "" };
            var json = JsonConvert.DeserializeAnonymousType(responseString, tokens);

            if (string.IsNullOrEmpty(json.access_token) || string.IsNullOrEmpty(json.refresh_token)) {
                _logger.LogWarning("GetTokens failed: {reply}", responseString);
                return string.Empty;
            }

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
            string token = null;

            if (HttpContext.Current.Request.Cookies["refresh_token"] != null)
                token = HttpContext.Current.Request.Cookies["refresh_token"].Value;

            if (token == null)
            {
                return string.Empty;
            }

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("client_id", _clientId));
            formData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));
            formData.Add(new KeyValuePair<string, string>("redirect_uri", GetRedirectUrl()));
            formData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            formData.Add(new KeyValuePair<string, string>("refresh_token", token));

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://login.live.com/oauth20_token.srf", new FormUrlEncodedContent(formData));
            var responseString = await response.Content.ReadAsStringAsync();

            var tokens = new { access_token = "", refresh_token = "" };
            var json = JsonConvert.DeserializeAnonymousType(responseString, tokens);

            if (string.IsNullOrEmpty(json.access_token) || string.IsNullOrEmpty(json.refresh_token)) {
                _logger.LogWarning("RefreshTokens failed: {reply}", responseString);
                return string.Empty;
            }

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

        public string GetLogoutUrl()
        {
            return _liveAuthClient.GetLogoutUrl(GetRedirectUrl());
        }
    }
}

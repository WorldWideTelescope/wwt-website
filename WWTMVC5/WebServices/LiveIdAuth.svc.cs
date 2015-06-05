using System;
using System.Configuration;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
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
        private LiveAuthClient _liveAuthClient = new LiveAuthClient(ConfigReader<string>.GetSetting("LiveClientId"), ConfigReader<string>.GetSetting("LiveClientSecret"), null);
        private LiveConnectSession _session = null;

        [WebGet]
        [OperationContract]
        public async Task<LiveLoginResult> Authenticate()
        {
            LiveLoginResult result = null;
            try
            {
                //string url = _liveAuthClient.GetLoginUrl(new string[] { "wl.signin", "wl.basic", "wl.emails"});
                result = await _liveAuthClient.InitializeWebSessionAsync(new HttpContextWrapper(HttpContext.Current));
                if (false)
                {
                    result = await _liveAuthClient.ExchangeAuthCodeAsync(new HttpContextWrapper(HttpContext.Current));
                }
                _session = result.Session;
            }
            catch (LiveAuthException){}
            return result;
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

// This file used to be the central hub through which most requests for
// user-facing HTML were routed. Now we've offloaded as much functionality as
// possible to other web frameworks, so this there are only a few routes that
// this controller now supports.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Live;
using Unity;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    public class DefaultController : ControllerBase
    {
        private ICommunityService _communityService;
        private INotificationService _notificationService;
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(IProfileService profileService, ICommunityService communityService,
                                 INotificationService queueService, ILogger<DefaultController> logger)
            : base(profileService)
        {
            _communityService = communityService;
            _notificationService = queueService;
            _logger = logger;
        }

        private readonly BaseModel _baseModel = new BaseModel();

        [AllowAnonymous]
        [Route("LiveId/Authenticate")]
        public async Task<JsonResult> Authenticate()
        {
            var profile = await TryAuthenticateFromHttpContext();
            if (profile != null)
            {
                _baseModel.User = profile;
                return Json(new
                {
                    Status = "Connected",
                    Session = new
                    {

                        User = SessionWrapper.Get<string>("CurrentUserProfileName")
                    },

                }, JsonRequestBehavior.AllowGet);
            }

            var svc = new LiveIdAuth();
            var url = svc.GetLogoutUrl();

            SessionWrapper.Clear();
            return Json(new
            {
                Status = "unknown",
                S = url
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("LiveId/AuthenticateFromCode/{code}")]
        public async Task<ActionResult> AuthenticateFromCode(string code)
        {
            var user = await TryAuthenticateFromAuthCode(code);
            _baseModel.User = user;
            string url = Uri.UnescapeDataString(Request.QueryString["returnUrl"]).ToLower();

            if (url.IndexOf("/community") != -1)
            {
                return Redirect("/Community");
            }

            if (url.IndexOf("/webclient") != -1)
            {
                return Redirect("/webclient/?loggedIn=true");
            }

            return Redirect("/home");
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            var svc = new LiveIdAuth();
            var url =  svc.GetLogoutUrl();

            SessionWrapper.Clear();

            var refreshTokenCookie = Response.Cookies["refresh_token"];
            var accessTokenCookie = Response.Cookies["access_token"];

            if (refreshTokenCookie != null && !string.IsNullOrEmpty(refreshTokenCookie.Value))
            {
                refreshTokenCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(refreshTokenCookie);
            }

            if (accessTokenCookie != null && !string.IsNullOrEmpty(accessTokenCookie.Value))
            {
                accessTokenCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(accessTokenCookie);
            }

            return Redirect(url);
        }

        private string MakeIndexUrl(string prefix, string page)
        {
            if (page.ToLower() == "index")
                return prefix;
            return prefix + page;
        }

        [Route("")]
        [Route("Index")]
        public async Task<ActionResult> RootIndexPage(string page)
        {
            return await ViewWithLogin("/", "~/Views/Index.cshtml");
        }

        [Route("Community/{page=Index}")]
        public async Task<ActionResult> CommunityPage(string page)
        {
            var reloadUrl = MakeIndexUrl("/Community/", page);
            var viewTarget = $"~/Views/Community/{page}.cshtml";
            ViewBag.page = page;
            return await ViewWithLogin(reloadUrl, viewTarget);
        }

        public async Task<ActionResult> ViewWithLogin(string reloadUrl, string viewTarget)
        {
            if (_baseModel.User == null)
            {
                _baseModel.User = await TryAuthenticateFromHttpContext();
            }

            if (_baseModel.User == null)
            {
                var code = "";

                if (!string.IsNullOrEmpty(Request.QueryString["code"]))
                    code = Request.QueryString["code"];

                // This call is a bit overloaded -- it will attempt to refresh
                // the auth using a refresh_token cookie if available, even if
                // there is no code.
                _baseModel.User = await TryAuthenticateFromAuthCode(code);

                if (!string.IsNullOrEmpty(code))
                    return Redirect(reloadUrl);
            }

            ViewBag.CurrentUserId = CurrentUserId;
            // Hopefully this is cheap to create? This value is needed to
            // properly wire up OAuth login in the user-facing HTML.
            var svc = new LiveIdAuth();
            ViewBag.LiveRedirectUrl = svc.GetRedirectUrl();
            return View(viewTarget, _baseModel);
        }
    }
}

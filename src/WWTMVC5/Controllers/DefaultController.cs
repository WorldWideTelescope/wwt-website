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
    [RoutePrefix("")]
    [Route("{action=Index}")]
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

        public async Task<ActionResult> Index()
        {
            return await GetViewOrRedirect(string.Empty, "index", _baseModel);
        }

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
            if (Request.Headers.Get("host").Contains("localhost"))
            {
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

                return Redirect("/home");
            }

            var user = await TryAuthenticateFromAuthCode(code);
            _baseModel.User = user;
            string url = Uri.UnescapeDataString(Request.QueryString["returnUrl"]).ToLower();

            if (url.IndexOf("/community") != -1)
            {
                return Redirect("/Community");
            }
            if (url.IndexOf("/webclient") != -1)
            {
                return Redirect("/webclient?loggedIn=true");
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

        [Route("{group}/{page=Index}")]
        public async Task<ActionResult> ViewResult(string group, string page)
        {
            try
            {
                if (group.ToLower() == "community" && page.ToLower() == "profile" && _baseModel.User == null)
                {
                    await TryAuthenticateFromHttpContext();

                    if (CurrentUserId != 0)
                    {
                        _baseModel.User = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
                        return await GetViewOrRedirect(group, page, _baseModel);
                    }

                    return Redirect("/Community");
                }

                ViewBag.page = page;
                ViewBag.group = group;
                ViewBag.CurrentUserId = CurrentUserId;
                return await GetViewOrRedirect(group, page, _baseModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ViewResult: error in dispatch");
                return View("~/Views/Support/Error.cshtml", _baseModel);
            }
        }

        private async Task<ActionResult> GetViewOrRedirect(string group, string page, BaseModel model)
        {
            if (model.User == null)
            {
                if (Request.QueryString["code"] != null)
                {
                    model.User = await TryAuthenticateFromAuthCode(Request.QueryString["code"]);

                    if (page == "index")
                    {
                        page = "";
                    }

                    var strippedUrl = group + "/" + page;

                    if (strippedUrl == "/") {
                        strippedUrl = "/home";
                    }

                    return Redirect(strippedUrl);
                }

                if (Request.Cookies["refresh_token"] != null)
                {
                    model.User = await TryAuthenticateFromAuthCode("");
                }
            }

            if (group == string.Empty) {
                var homeCookie = Request.Cookies["homepage"];
                var rootDir = "webclient";

                if (homeCookie != null && !string.IsNullOrEmpty(homeCookie.Value)) {
                    rootDir = homeCookie.Value;
                }

                return Redirect(rootDir);
            }

            if (group.ToLower() == "home") {
                return View("~/Views/index.cshtml", model);
            }

            return View("~/Views/" + group + "/" + page + ".cshtml", model);
        }
    }
}

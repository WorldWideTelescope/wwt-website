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
        /// <summary>
        /// Instance of Queue Service
        /// </summary>
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

        private static readonly string[] ViewGroups = new string[]
        {
            "about",
            "community",
            "download",
            "getinvolved",
            "home",
            "learn",
            "profile",
            "support",
            "terms",
            "upgrade",
            "use"
        };
    
        public async Task<ActionResult> Index()
        {
            _logger.LogInformation("DefaultController Index handler");
            return await GetViewOrRedirect(string.Empty,"index", _baseModel);
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
            var url = svc.GetLogoutUrl("http://" + Request.Headers.Get("host"));

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
            _logger.LogInformation("Handler AuthFromCode: code = {code}", code);

            if (Request.Headers.Get("host").Contains("localhost"))
            {
                _logger.LogInformation("... localhost branch");
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
            _logger.LogInformation("AuthFromCode: trying to get user");
            var user = await TryAuthenticateFromAuthCode(code);
            _logger.LogInformation("AuthFromCode: got user with email {email}", user.Email);
            _baseModel.User = user;
            string url = Uri.UnescapeDataString(Request.QueryString["returnUrl"]).ToLower();
            _logger.LogInformation("AuthFromCode: redirecting with URL {url}", url);
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
            var url =  svc.GetLogoutUrl("http://" + Request.Headers.Get("host")); 
            
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
            _logger.LogInformation("Handler ViewResult: group = {g}, page = {p}", group, page);

            try
            {
                if (!ViewGroups.Contains(group.ToLower()))
                {
                    _logger.LogInformation("ViewResult: early error");
                    return View("~/Views/Support/Error.cshtml", _baseModel);
                }
                if (group.ToLower() == "wwtstories")
                {
                    return Redirect("http://wwtstories.org");
                }
                if (page.Contains(".msi") || (page.ToLower() == "error" && Request.RawUrl.Contains(".msi")))
                {
                    _logger.LogInformation("ViewResult: MSI");
                    return await GetViewOrRedirect("download","index", _baseModel);
                }
                if (group.ToLower() == "community" && page.ToLower() == "profile" && _baseModel.User == null)
                {
                    _logger.LogInformation("ViewResult: community branch");
                    await TryAuthenticateFromHttpContext();
                    if (CurrentUserId != 0)
                    {
                        _baseModel.User = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
                        return await GetViewOrRedirect(group, page, _baseModel);
                    }
                    
                    return Redirect("/Community");
                }
                _logger.LogInformation("ViewResult: default branch");
                ViewBag.page = page = GetQsPage(page);
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
        
        /// <summary>
        /// All web page views go through this function - it ensures we are not in openwwt land - which should only display a kiosk
        /// </summary>
        /// <param name="group"></param>
        /// <param name="page"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ActionResult> GetViewOrRedirect(string group, string page, BaseModel model)
        {
            _logger.LogInformation("GetViewOrRedirect: group = {g}, page = {p}", group, page);
            model.IsOpenWwtKiosk = Request.Headers.Get("host").ToLower().Contains("openwwt.org");

            if (model.IsOpenWwtKiosk && group.ToLower() != "openwwt")
            {
                group = "openwwt";
                page = "index";
            }
            if (model.User == null)
            {
                _logger.LogInformation("GetViewOrRedirect: null user");
                if (Request.QueryString["code"] != null)
                {
                    _logger.LogInformation("GetViewOrRedirect: trying code auth with code = {c}", Request.QueryString["code"]);
                    model.User = await TryAuthenticateFromAuthCode(Request.QueryString["code"]);
                    if (page == "index")
                    {
                        page = "";
                    }
                    var strippedUrl = group + "/" + page;
                    if (strippedUrl == "/") {
                        strippedUrl = "/home";
                    }
                    //redirect strips gnarly looking code from qs
                    _logger.LogInformation("GetViewOrRedirect: auth OK; redirect: {u}", strippedUrl);
                    return Redirect(strippedUrl);
                }
                if (Request.Cookies["refresh_token"] != null)
                {
                    _logger.LogInformation("GetViewOrRedirect: refresh_token present; trying blank code");
                    model.User = await TryAuthenticateFromAuthCode("");
                }
            }
            if (group == string.Empty) {
                _logger.LogInformation("GetViewOrRedirect: empty group redirect");
                var homeCookie = Request.Cookies["homepage"];
                var rootDir = homeCookie == null || string.IsNullOrEmpty(homeCookie.Value) ? "webclient" : homeCookie.Value;
                return Redirect(rootDir);
            }
            _logger.LogInformation("GetViewOrRedirect: default redirect");
            return group.ToLower() == "home" ? View("~/Views/index.cshtml", model) : View("~/Views/" + group + "/" + page + ".cshtml", model);
        }

        //Ensure old webform routes still return the proper view
        private string GetQsPage(string page)
        {
            if (page == "Index" && Request.QueryString.Count == 1 && Request.QueryString["code"] == null)
            {
                page = Request.QueryString.Get(0);
            }
            return page;
        }

    }
}

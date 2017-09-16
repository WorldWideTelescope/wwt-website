using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Live;
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
        public DefaultController(IProfileService profileService, ICommunityService communityService, INotificationService queueService)
            : base(profileService)
        {
            _communityService = communityService;
            _notificationService = queueService;
        }
        private readonly BaseModel _baseModel = new BaseModel();

        private static readonly string[] ViewGroups = new string[]
        {
            "about",
            "blog",
            "community",
            "developers",
            "download",
            "educators",
            "explorers",
            "eyewire",
            "getinvolved",
            "home",
            "interact",
            "layerscape",
            "learn",
            "museums",
            "news",
            "openwwt",
            "planetariums",
            "profile",
            "researchers",
            "support",
            "terms",
            "upgrade",
            "use",
            "wwtinaction",
            "wwtstories"
        };
    
        public async Task<ActionResult> Index()
        {
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
            try
            {
                if (!ViewGroups.Contains(group.ToLower()))
                {
                    return View("~/Views/Support/Error.cshtml", _baseModel);
                }
                if (group.ToLower() == "wwtstories")
                {
                    return Redirect("http://wwtstories.org");
                }
                if (page.Contains(".msi") || (page.ToLower() == "error" && Request.RawUrl.Contains(".msi")))
                {
                    return await GetViewOrRedirect("download","index", _baseModel);
                }
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
                ViewBag.page = page = GetQsPage(page);
                ViewBag.group = group;
                ViewBag.CurrentUserId = CurrentUserId;

                return await GetViewOrRedirect(group, page, _baseModel);
            }
            catch (Exception ex)
            {
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
            model.IsOpenWwtKiosk = Request.Headers.Get("host").ToLower().Contains("openwwt.org");

            

            if (model.IsOpenWwtKiosk && group.ToLower() != "openwwt")
            {
                group = "openwwt";
                page = "index";
            }
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
                    //redirect strips gnarly looking code from qs
                    return Redirect(strippedUrl);
                }
                if (Request.Cookies["refresh_token"] != null)
                {
                    model.User = await TryAuthenticateFromAuthCode("");
                }
            }
            if (group == string.Empty) {
                var homeCookie = Request.Cookies["homepage"];
                var rootDir = homeCookie == null || string.IsNullOrEmpty(homeCookie.Value) ? "webclient" : homeCookie.Value;
                return Redirect(rootDir);
            }
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

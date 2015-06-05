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
            "community",
            "developers",
            "download",
            "educators",
            "explorers",
            "eyewire",
            "interact",
            "layerscape",
            "learn",
            "museums",
            "openwwt",
            "planetariums",
            "profile",
            "researchers",
            "support",
            "terms",
            "wwtinaction",
            "wwtstories"
        };
    
        public ActionResult Index()
        {
            return GetViewOrRedirect(string.Empty,"index", _baseModel);
        }

        [AllowAnonymous]
        [Route("LiveId/Authenticate")]
        public async Task<JsonResult> Authenticate()
        {
            LiveLoginResult result = await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            if (result.Status == LiveConnectSessionStatus.Connected){    
                return Json(new
                {
                    Status = result.Status.ToString(), 
                    Session = new
                    {
                        result.Session.AccessToken,
                        result.Session.AuthenticationToken,
                        Expires = result.Session.Expires.ToLocalTime().ToString(),
                        result.Session.RefreshToken,
                        result.Session.Scopes
                    },
                   
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Status = result.Status.ToString()
                });
            }
            
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            var svc = SessionWrapper.Get<LiveIdAuth>("LiveAuthSvc");
            var url = "/";
            if (svc != null) 
            {
                url = svc.GetLogoutUrl("http://" + Request.Headers.Get("host")); 
            }
            SessionWrapper.Clear();
            return Redirect(url); //View("~/Views/Index.cshtml", baseModel);
        }

        [Route("{group}/{page=Index}")]
        public ActionResult ViewResult(string group, string page)
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
                    return GetViewOrRedirect("download","index", _baseModel);
                }
                if (group.ToLower() == "community" && page == "profile" && _baseModel.User == null)
                {
                    return Redirect("/Community");
                }
                ViewBag.page = page = GetQsPage(page);
                ViewBag.group = group;
                ViewBag.CurrentUserId = CurrentUserId;

                return GetViewOrRedirect(group, page, _baseModel);
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
        private ActionResult GetViewOrRedirect(string group, string page, BaseModel model)
        {
            model.IsOpenWwtKiosk = Request.Headers.Get("host").ToLower().Contains("openwwt.org");

            if (model.IsOpenWwtKiosk && group.ToLower() != "openwwt")
            {
                group = "openwwt";
                page = "index";
            }
            return group == string.Empty ? View("~/Views/index.cshtml", model) : View("~/Views/" + group + "/" + page + ".cshtml", model);
        }

        //Ensure old webform routes still return the proper view
        private string GetQsPage(string page)
        {
            if (page == "Index" && Request.QueryString.Count == 1)
            {
                page = Request.QueryString.Get(0);
            }
            return page;
        }

    }
}

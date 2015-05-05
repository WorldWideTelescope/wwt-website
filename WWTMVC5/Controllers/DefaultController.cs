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
        private ICommunityService communityService;
        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService notificationService;
        public DefaultController(IProfileService profileService, ICommunityService communityService, INotificationService queueService)
            : base(profileService)
        {
            this.communityService = communityService;
            this.notificationService = queueService;
        }
        private readonly BaseModel baseModel = new BaseModel();

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
            "wwtinaction",
            "wwtstories"
        };
        
    
        public ActionResult Index()
        {
            return GetViewOrRedirect("~/Views/Index.cshtml", baseModel, "");
        }


        [AllowAnonymous]
        [Route("LiveId/Authenticate")]
        public async Task<JsonResult> Authenticate()
        {
            var svc = new LiveIdAuth();
            LiveLoginResult result = await svc.Authenticate();
            if (result.Status == LiveConnectSessionStatus.Connected)
            {
                var client = new LiveConnectClient(result.Session);
                SessionWrapper.Set("LiveConnectClient", client);
                SessionWrapper.Set("LiveConnectResult", result);
                SessionWrapper.Set("LiveAuthSvc", svc);
                
                var getResult = await client.GetAsync("me");
                var jsonResult = getResult.Result as dynamic;
                var profileDetails = ProfileService.GetProfile(jsonResult.id);
                if (profileDetails == null)
                {
                    profileDetails = new ProfileDetails(jsonResult);
                    // While creating the user, IsSubscribed to be true always.
                    profileDetails.IsSubscribed = true;

                    // When creating the user, by default the user type will be of regular. 
                    profileDetails.UserType = UserTypes.Regular;
                    profileDetails.ID = this.ProfileService.CreateProfile(profileDetails);
                    
                    // This will used as the default community when user is uploading a new content.
                    // This community will need to have the following details:
                    CommunityDetails communityDetails = new CommunityDetails
                    {
                        CommunityType = CommunityTypes.User,// 1. This community type should be User
                        CreatedByID = profileDetails.ID,// 2. CreatedBy will be the new USER.
                        IsFeatured = false,// 3. This community is not featured.
                        Name = Resources.UserCommunityName,// 4. Name should be NONE.
                        AccessTypeID = (int) AccessType.Private,// 5. Access type should be private.
                        CategoryID = (int) CategoryType.GeneralInterest// 6. Set the category ID of general interest. We need to set the Category ID as it is a foreign key and cannot be null.
                    };

                    // 7. Create the community
                    communityService.CreateCommunity(communityDetails);

                    // Send New user notification.
                    this.notificationService.NotifyNewEntityRequest(profileDetails,
                        HttpContext.Request.Url.GetServerLink());
                }
                
                SessionWrapper.Set<long>("CurrentUserID", profileDetails.ID);
                SessionWrapper.Set<string>("CurrentUserProfileName", profileDetails.FirstName + " " + profileDetails.LastName);
                SessionWrapper.Set("ProfileDetails", profileDetails);
                SessionWrapper.Set("AuthenticationToken", result.Session.AuthenticationToken);
                return Json(new
                {
                    Status = result.Status.ToString(), 
                    //State = result.State != null ? result.State.ToString() : null,
                    Session = new
                    {
                        result.Session.AccessToken,
                        result.Session.AuthenticationToken,
                        Expires = result.Session.Expires.ToLocalTime().ToString(),
                        result.Session.RefreshToken,
                        result.Session.Scopes
                    },
                    profile=jsonResult
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
                    return GetViewOrRedirect("~/Views/Support/Error.cshtml", baseModel, "Support/Error");
                }
                if (group.ToLower() == "wwtstories")
                {
                    return Redirect("http://wwtstories.org");
                }
                if (page.Contains(".msi") || (page.ToLower() == "error" && Request.RawUrl.Contains(".msi")))
                {
                    return GetViewOrRedirect("~/Views/Download/Index.cshtml", baseModel, "/Download");
                }
                if (group.ToLower() == "community" && page == "profile" && baseModel.User == null)
                {
                    return Redirect("/");
                }
                ViewBag.page = page = GetQsPage(page);
                ViewBag.group = group;
                ViewBag.CurrentUserId = CurrentUserID;

                return GetViewOrRedirect("~/Views/" + group + "/" + page + ".cshtml", baseModel, group + "/" + page);
            }
            catch (Exception ex)
            {
                return GetViewOrRedirect("~/Views/Support/Error.cshtml", baseModel, "Support/Error");
            }
        }

        private ActionResult GetViewOrRedirect(string viewPath, BaseModel model, string redirectPath)
        {
            if (model.Staging || Request.Headers.Get("host").ToLower().Contains("openwwt"))
            {
                return View(viewPath, model);
            }
            else
            {
                return Redirect("http://openwwt.org/" + redirectPath);
            }
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

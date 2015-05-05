//-----------------------------------------------------------------------
// <copyright file="AdminController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the AdminController class.
        /// </summary>
        /// <param name="profileService">Instance of profile Service</param>
        public AdminController(IProfileService profileService)
            : base(profileService)
        {
        }

        [HttpPost]
        [Route("Admin/Content/Approve/{id}")]
        public bool ApproveContent(string id)
        {
            var result = false;
            if (IsSiteAdmin)
            {
                var adminsvc = new AdministrationService();
                result = adminsvc.MarkAsPublicContent(id);
            }
            return result;
        }

        /// <summary>
        /// Index Action which is default action rendering the admin page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        [HttpGet]
        public ActionResult Index()
        {
            CheckIfSiteAdmin();

            return View();
        }

        /// <summary>
        /// Action for loading the page requested by the user.
        /// </summary>
        /// <param name="id">Start page which has to be loaded.</param>
        /// <param name="name">Entity type name.</param>
        /// <returns>Returns the View to be used</returns>
        [HttpGet]
        
        public ActionResult Action(string id, string name)
        {
            CheckIfSiteAdmin();

            ViewData["StartPage"] = id ?? string.Empty;
            ViewData["EntityType"] = name ?? string.Empty;
            ViewData["AuthToken"] = HttpContext.GetAuthStateToken();
            ViewData["AuthTokenType"] = HttpContext.GetAuthStateTokenType();

            return View();
        }
    }
}
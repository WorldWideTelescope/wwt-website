//-----------------------------------------------------------------------
// <copyright file="SocialSharingController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the social sharing partial view request which creates the URL for sharing
    /// current community or content.
    /// </summary>
    public class SocialSharingController : Controller
    {
        /// <summary>
        /// Index Action which is default action rendering the social sharing content.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}

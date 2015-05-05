//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the home page request which makes request to repository and get the
    /// required data and pushes them to the View.
    /// </summary>
    public class HomeController : ControllerBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        /// <param name="profileService">Instance of profile Service</param>
        public HomeController(IProfileService profileService)
            : base(profileService)
        {
        }

        #endregion Constructor

        /// <summary>
        /// Startup Action which is default action rendering the home page. This needs to be added because of the
        /// issue with WindowsLiveIDAuthenticationModule when a non-fully qualified URL is used which would be the case with
        /// the default action
        /// Additional details at http://sharepoint/sites/liveid/wiki/Wiki%20Pages/AuthModPathlessURLs.aspx
        /// </summary>
        /// <returns>Redirects to Index action</returns>
        public ActionResult Startup()
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Index Action for rendering the home page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// FAQs Action for rendering the FAQ page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult FAQs()
        //{
        //    return View();
        //}

        /// <summary>
        /// Excel Add-in Help Action for rendering the ExcelAddInHelp page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        public ActionResult ExcelAddInHelp()
        {
            return View();
        }

        /// <summary>
        /// InstallWWT Action for rendering the Install page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult InstallWWT()
        //{
        //    return View();
        //}

        /// <summary>
        /// ExcelAddInWelcome Action for rendering the ExcelAddInWelcome page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult ExcelAddInWelcome()
        //{
        //    return View();
        //}

        /// <summary>
        /// LearnMore Action for rendering the Learn More page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult LearnMore()
        //{
        //    return View();
        //}

        /// <summary>
        /// HelpLink Action for rendering the Help links for the master page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult HelpLink()
        //{
        //    return PartialView("HelpLinkView", this.IsSiteAdmin);
        //}

        /// <summary>
        /// GetStarted Action for rendering the Get Started page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult GetStarted()
        //{
        //    return View();
        //}

        /// <summary>
        /// VisualizingContentinWWT Action for rendering the Get Started page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult VisualizingContentinWWT()
        //{
        //    return View();
        //}

        /// <summary>
        /// Narwhal Action for rendering the Get Started page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult Narwhal()
        //{
        //    return View();
        //}

        /// <summary>
        /// WWTAddinForExcel Action for rendering the Get Started page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        //public ActionResult WWTAddinForExcel()
        //{
        //    return View();
        //}
    }
}
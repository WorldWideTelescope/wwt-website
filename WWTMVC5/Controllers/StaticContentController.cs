//-----------------------------------------------------------------------
// <copyright file="StaticContentController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the static content view request which makes request to repository and gets/publishes the
    /// required data about static content and pushes them to the View.
    /// </summary>
    public class StaticContentController : ControllerBase
    {
        #region Member variables

        /// <summary>
        /// Instance of Static Content Service
        /// </summary>
        private IStaticContentService _staticContentService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the StaticContentController class.
        /// </summary>
        /// <param name="staticContentService">Instance of Rating Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public StaticContentController(IStaticContentService staticContentService, IProfileService profileService)
            : base(profileService)
        {
            this._staticContentService = staticContentService;
        }

        #endregion

        #region Action Methods

        [HttpGet]
        
        public ActionResult Edit()
        {
            CheckIfSiteAdmin();

            var staticContentViewModel = new StaticContentViewModel();
            return View("Save", staticContentViewModel);
        }

        /// <summary>
        /// Fills the view on the basis of content type
        /// </summary>
        /// <param name="contentType">The static content type</param>
        /// <returns>Edit view for static content</returns>
        [HttpPost]
        
        public ActionResult Edit(StaticContentType contentType)
        {
            CheckIfSiteAdmin();

            var staticContentViewModel = new StaticContentViewModel();
            var staticContent = this._staticContentService.GetStaticContent(contentType);
            staticContentViewModel.Content = staticContent.Content;

            try
            {
                // Format the HTML returned by Telerik control so that indented HTML will be shown in HTML editor.
                staticContentViewModel.Content = XElement.Parse(staticContentViewModel.Content).ToString();
            }
            catch (XmlException)
            {
                try
                {
                    // In case if the HTML doesn't have a root, add a <p> tag as wrapper.
                    staticContentViewModel.Content = XElement.Parse(string.Format(CultureInfo.CurrentCulture, "<p>{0}</p>", staticContentViewModel.Content)).ToString();
                }
                catch (XmlException)
                {
                    // Consume any other Xml exception with parsing and try to load the string as is.
                    // There could be problem like the text is not a valid XML.
                }
            }

            return View("Save", staticContentViewModel);
        }

        /// <summary>
        /// Saves the static content details
        /// </summary>
        /// <param name="staticContentViewModel">The model with properties for static content</param>
        /// <returns>Action result</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "TODO: Custom Exception handling to be added.")]
        public ActionResult Save(StaticContentViewModel staticContentViewModel)
        {
            CheckIfSiteAdmin();

            // Make sure staticContentViewModel is not null
            this.CheckNotNull(() => new { staticContentViewModel });
            if (ModelState.IsValid)
            {
                var staticContent = new StaticContentDetails()
                {
                    TypeID = staticContentViewModel.ContentType,
                    Content = staticContentViewModel.Content,
                    ModifiedByID = this.CurrentUserId
                };

                OperationStatus status = this._staticContentService.UpdateStaticContent(staticContent);
                if (!status.Succeeded)
                {
                    throw new Exception(status.ErrorMessage, status.Exception);
                }

                switch (staticContentViewModel.ContentType)
                {
                    case (int)StaticContentType.HomePageHelpText:
                        return RedirectToAction("Index", "Home");
                    case (int)StaticContentType.FAQ:
                        return RedirectToAction("FAQs", "Home");
                    case (int)StaticContentType.WWTInstall:
                        return RedirectToAction("InstallWWT", "Home");
                    case (int)StaticContentType.ExcelInstall:
                        return RedirectToAction("ExcelAddInWelcome", "Home");
                    case (int)StaticContentType.ExcelHelp:
                        return RedirectToAction("ExcelAddInHelp", "Home");
                    case (int)StaticContentType.LearnMore:
                        return RedirectToAction("LearnMore", "Home");
                    case (int) StaticContentType.GetStarted:
                        return RedirectToAction("GetStarted", "Home");
                    case (int)StaticContentType.VisualizingContentinWWT:
                        return RedirectToAction("VisualizingContentinWWT", "Home");
                    case (int)StaticContentType.Narwhal:
                        return RedirectToAction("Narwhal", "Home");
                    case (int)StaticContentType.WWTAddinForExcel:
                        return RedirectToAction("WWTAddinForExcel", "Home");
                    default:
                        return RedirectToAction("Index", "Admin");
                }
            }
            else
            {
                // In case of any validation error stay in the same page.
                staticContentViewModel.Content = Server.HtmlDecode(staticContentViewModel.Content);
                return View("Save", staticContentViewModel);
            }
        }

        /// <summary>
        /// It renders the static content partial view
        /// </summary>
        /// <param name="staticContentType">static Content Type</param>
        [HttpGet]
        [ChildActionOnly]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception handling for partial views")]
        public void Render(StaticContentType staticContentType)
        {
            try
            {
                var staticContent = this._staticContentService.GetStaticContent(staticContentType);
                PartialView("StaticContentView", staticContent).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Preview Action for rendering the Preview page.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        public ActionResult Preview()
        {
            return View();
        }

        #endregion
    }
}
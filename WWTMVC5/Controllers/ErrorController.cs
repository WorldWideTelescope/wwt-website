//-----------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using WWTMVC5.Properties;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the errors thrown in the application and showing appropriate error messages.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Action handling the general error messages.
        /// </summary>
        /// <returns>General view</returns>
        public ActionResult General()
        {
            string errorMessage = TempData["ErrorMessage"] as string;
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                errorMessage = Resources.GeneralErrorMessage;
            }

            // There is another overloaded constructor for View, which takes string as parameter and considers that as view name.
            // To pass string value for Model, need to use the parameter type as OBJECT only.
            return View(errorMessage as object);
        }

        /// <summary>
        /// Action handling the not found error messages.
        /// </summary>
        /// <returns>Not Found view</returns>
        public ActionResult NotFound()
        {
            return View();
        }

        /// <summary>
        /// Action handling the unauthorized error messages.
        /// </summary>
        /// <returns>Unauthorized view</returns>
        public ActionResult Unauthorized(string errorMessage)
        {
            return View(errorMessage as object);
        }

        /// <summary>
        /// Action handling the invalid request error messages.
        /// </summary>
        /// <returns>Invalid request view</returns>
        public ActionResult Invalid(string errorMessage)
        {
            return View(errorMessage as object);
        }
    }
}

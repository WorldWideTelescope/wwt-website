//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using WWTMVC5.Extensions;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the category page request which makes request to repository and get the
    /// required data about category selected and pushes them to the View.
    /// </summary>
    public class CategoryController : Controller
    {
        /// <summary>
        /// Index Action which is default action rendering the category page.
        /// </summary>
        /// <param name="id">Category name to be processed</param>
        /// <returns>Returns the View to be used</returns>
        /// <remarks>Using id as parameter to avoid one more route</remarks>
        public ActionResult Index(int id)
        {
            var categoryName = CategoryType.All;

            // Get the categoryName from the Enum based on the index.
            categoryName = id.ToEnum<int, CategoryType>(CategoryType.All);

            // There is another overloaded constructor for View, which takes string as parameter and considers that as view name.
            // To pass the value for Model, need to use the parameter type as OBJECT only.
            return View(categoryName.ToString() as object);
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the search partial view request which makes request to repository and gets/publishes the
    /// required data about search page and pushes them to the View.
    /// </summary>
    [ValidateInput(false)]
    public class SearchController : ControllerBase
    {
        /// <summary>
        /// Instance of Search Service
        /// </summary>
        private ISearchService searchService;

        /// <summary>
        /// Initializes a new instance of the SearchController class.
        /// </summary>
        /// <param name="searchService">Instance of Search Service</param>
        /// <param name="profileService"> Instance of a profile service. </param>
        public SearchController(ISearchService searchService, IProfileService profileService)
            : base(profileService)
        {
            this.searchService = searchService;
        }

        /// <summary>
        /// Gets the user's selected search type
        /// </summary>
        public static string SelectedSearchType
        {
            get
            {
                return SessionWrapper.Get<string>("SelectedSearchType", Constants.BasicSearchType);
            }
        }

        /// <summary>
        /// Search the content
        /// </summary>
        /// <returns>The search view Results.aspx</returns>
        public ActionResult Results(string searchText)
        {
            // To pass the value for Model, need to use the parameter type as OBJECT only.
            return View(searchText as object);
        }

        /// <summary>
        /// This returns the partial view of search box
        /// </summary>
        /// <param name="searchText">search Text</param>
        [ChildActionOnly]
        public void Render(string searchText)
        {
            try
            {
                // There is another overloaded constructor for View, which takes string as parameter and considers that as view name.
                // To pass the value for Model, need to use the parameter type as OBJECT only.
                PartialView("SearchView", searchText as object).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        ///// <summary>
        ///// This returns the Search Result View depending on the selected search type
        ///// </summary>
        ///// <param name="searchText">search Text</param>
        //[ChildActionOnly]
        //public void RenderResults(string searchText)
        //{
        //    try
        //    {
        //        // It creates the prefix for id of links
        //        SetSiteAnalyticsPrefix(HighlightType.None);

        //        SearchViewModel searchQuery = new SearchViewModel();
        //        searchQuery.ResultsPerPage = (int)SearchResultsPerPage.Ten;

        //        searchQuery.Results = GetSearchResults(searchText, SelectedSearchType, 1, searchQuery);
        //        PartialView("SearchResultView", searchQuery).ExecuteResult(this.ControllerContext);
        //    }
        //    catch (Exception)
        //    {
        //        // Consume the exception and render rest of the views in the page.
        //        // TODO: Log the exception?
        //    }
        //}

        /// <summary>
        /// This returns the Search Result View depending on the selected search type for Ajax calls
        /// </summary>
        /// <param name="searchText">search Text</param>
        /// <param name="currentPage"></param>
        
        [HttpPost]
        [Route("Search/RenderJson/{searchText}/{categories}/{contentTypes}/{currentPage}/{pageSize}")]
        public JsonResult AjaxRenderResults(string searchText, string categories, string contentTypes, int currentPage, int pageSize)
        {

            if (categories == "0")
            {
                categories = null;
            }
            if (contentTypes == "0")
            {
                contentTypes = null;
            }
            var searchQuery = new SearchViewModel {ResultsPerPage = pageSize,CategoryFilter = categories, ContentTypeFilter = contentTypes};
            // It creates the prefix for id of links
            SetSiteAnalyticsPrefix(HighlightType.None);
            SessionWrapper.Set<string>("SelectedSearchType", "basic");
            var results = GetSearchResults(searchText, "basic", currentPage, searchQuery);
            return Json(results);
        }

        /// <summary>
        /// This returns the results for Search Result View
        /// </summary>
        /// <param name="searchText">search Text</param>
        /// <param name="selectedTab">selected Tab Text</param>
        /// <returns>EntityViewModel collection</returns>
        private JsonResult GetSearchResults(string searchText, string selectedTab, int currentPage, SearchViewModel searchQuery, SearchSortBy sortBy = SearchSortBy.Rating)
        {
            ViewData["SearchText"] = searchText = (string.IsNullOrWhiteSpace(searchText) || searchText.ToLower(CultureInfo.CurrentCulture).Equals(Resources.DefaultSearchText.ToLower(CultureInfo.CurrentCulture))) ? string.Empty : searchText;
            ViewData["SearchMessage"] = string.Empty;
            IEnumerable<EntityViewModel> results = null;
            PageDetails pageDetails = new PageDetails(currentPage);
            
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    
                    pageDetails.ItemsPerPage = searchQuery.ResultsPerPage;

                    SearchQueryDetails searchQueryDetails = new SearchQueryDetails();

                    if (searchQuery.ContentTypeFilter != null)
                    {
                        foreach (var contentFilterValue in searchQuery.ContentTypeFilter.Split(','))
                        {
                            searchQueryDetails.ContentTypeFilter.Add(Convert.ToInt32(contentFilterValue, CultureInfo.CurrentCulture));
                        }
                    }

                    if (searchQuery.CategoryFilter != null)
                    {
                        foreach (var categoryFilterValue in searchQuery.CategoryFilter.Split(','))
                        {
                            searchQueryDetails.CategoryFilter.Add(Convert.ToInt32(categoryFilterValue, CultureInfo.CurrentCulture));
                        }
                    }

                    searchQueryDetails.SortBy = searchQuery.SortBy.ToEnum<string, SearchSortBy>(sortBy);

                    results = this.searchService.SimpleSearch(searchText.Replace("'", "''"), this.CurrentUserID, pageDetails, searchQueryDetails);

                    // If the total count of items are less than the selected per page items, select previous per page items
                    //ViewData["CurrentPage"] = currentPage;
                    //ViewData["TotalPage"] = pageDetails.TotalPages;
                    //ViewData["TotalCount"] = pageDetails.TotalCount;
                }
                

            return new JsonResult{Data = new
            {
                searchResults=results,
                pageInfo = pageDetails
            }};
        }
    }
}

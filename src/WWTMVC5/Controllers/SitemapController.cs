//-----------------------------------------------------------------------
// <copyright file="SitemapController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using WWTMVC5.Extensions;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Entity page request which makes request to repository and get the
    /// data about the either content or community and pushes them to the Entity View.
    /// </summary>
    public class SitemapController : ControllerBase
    {
        #region Members

        /// <summary>
        /// Instance of Community Service
        /// </summary>
        private ICommunityService _communityService;

        /// <summary>
        /// Instance of Content Service
        /// </summary>
        private IContentService _contentService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SitemapController class.
        /// </summary>
        /// <param name="communityService">Instance of community Service</param>
        /// <param name="contentService">Instance of content Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public SitemapController(ICommunityService communityService, IContentService contentService, IProfileService profileService)
            : base(profileService)
        {
            _communityService = communityService;
            _contentService = contentService;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action for handling the sitemap request.
        /// </summary>
        public ContentResult Index()
        {
            var dailyUrls = new List<string>();
            dailyUrls.Add(GetUrl(new { controller = "Home", action = "Index" }));
            foreach (var item in CategoryType.All.ToSelectList(CategoryType.All))
            {
                dailyUrls.Add(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", GetUrl(new { controller = "Category", action = "Index" }), item.Value));
            }

            var weeklyUrl = new List<string>();
            weeklyUrl.Add(GetUrl(new { controller = "Home", action = "FAQs" }));
            weeklyUrl.Add(GetUrl(new { controller = "Home", action = "InstallWWT" }));
            weeklyUrl.Add(GetUrl(new { controller = "Home", action = "ExcelAddInWelcome" }));

            var communityList = _communityService.GetLatestCommunityIDs(Constants.SitemapCount);
            foreach (var item in communityList)
            {
                weeklyUrl.Add(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", GetUrl(new { controller = "Community", action = "Index" }), item));
            }

            var contentList = _contentService.GetLatestContentIDs(Constants.SitemapCount);
            foreach (var item in contentList)
            {
                weeklyUrl.Add(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", GetUrl(new { controller = "Content", action = "Index" }), item));
            }

            var profileList = ProfileService.GetLatestProfileIDs(Constants.SitemapCount);
            foreach (var item in profileList)
            {
                weeklyUrl.Add(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", GetUrl(new { controller = "Profile", action = "Index" }), item));
            }

            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            ////Build the SiteMap
            var sitemap = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(xmlns + "urlset", from i in dailyUrls select new XElement(xmlns + "url", new XElement(xmlns + "loc", i), new XElement(xmlns + "lastmod", string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", DateTime.Now)), new XElement(xmlns + "changefreq", "daily"), new XElement(xmlns + "priority", "1"))));

            sitemap.Root.Add(from i in weeklyUrl select new XElement(xmlns + "url", new XElement(xmlns + "loc", i), new XElement(xmlns + "lastmod", string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", DateTime.Now)), new XElement(xmlns + "changefreq", "weekly"), new XElement(xmlns + "priority", "0.5")));

            return Content(sitemap.Declaration.ToString() + sitemap.ToString(), "text/xml");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the URL from the route values
        /// </summary>
        /// <param name="routeValues">Route value from which URL has to be obtained</param>
        /// <returns>URL obtained from route value</returns>
        private string GetUrl(object routeValues)
        {
            var values = new RouteValueDictionary(routeValues);
            var context = new RequestContext(HttpContext, RouteData);
            var url = RouteTable.Routes.GetVirtualPath(context, values).VirtualPath;
            return new Uri(Request.Url, url).AbsoluteUri;
        }

        #endregion
    }
}

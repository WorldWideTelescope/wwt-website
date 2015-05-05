//-----------------------------------------------------------------------
// <copyright file="SearchViewRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Search View repository having methods for retrieving community/content details
    /// from SQL Azure Layerscape database for searching.
    /// </summary>
    public class SearchViewRepository : RepositoryBase<SearchView>, ISearchViewRepository
    {
        /// <summary>
        /// Initializes a new instance of the SearchViewRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Instance of Layerscape db context</param>
        public SearchViewRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets the count of items which are satisfying the given search condition.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Number of items satisfying the given search condition</returns>
        public int SearchCount(string searchText, long? userId, SearchQueryDetails searchQueryDetails)
        {
            StringBuilder query = new StringBuilder("SELECT COUNT(*) FROM SearchView WHERE ");

            // Gets the WHERE clause for the search query
            query = query.Append(GetSearchQueryConditions(searchText, userId, searchQueryDetails));

            return this.EarthOnlineDbContext.Database.SqlQuery<int>(query.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// Searches the communities and contents and returns the items which are satisfying the given search condition.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped, for pagination</param>
        /// <param name="takeCount">Items to be taken, for pagination</param>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Items satisfying the given search condition</returns>
        public IEnumerable<SearchView> Search(string searchText, long? userId, int skipCount, int takeCount, SearchQueryDetails searchQueryDetails)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM SearchView WHERE ");

            // Gets the WHERE clause for the search query
            query = query.Append(GetSearchQueryConditions(searchText, userId, searchQueryDetails));

            // Gets the ORDER BY clause for the search query
            query = query.Append(GetSearchOrderBy(searchQueryDetails));

            return this.EarthOnlineDbContext.Database.SqlQuery<SearchView>(query.ToString()).Skip(skipCount).Take(takeCount).ToList();
        }

        /// <summary>
        /// Gets the WHERE clause for the select query to search communities and contents.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Where condition to be used in the query</returns>
        private static string GetSearchQueryConditions(string searchText, long? userId, SearchQueryDetails searchQueryDetails)
        {
            StringBuilder queryCondition = new StringBuilder(
                string.Format(CultureInfo.InvariantCulture, "(Name LIKE '%{0}%' OR Description LIKE '%{1}%' OR DistributedBy LIKE '%{2}%' OR Producer LIKE '%{3}%' OR Tags LIKE '%{4}%' OR Citation LIKE '%{5}%')", searchText, searchText, searchText, searchText, searchText, searchText));

            if (userId != null)
            {
                queryCondition = queryCondition.Append(string.Format(CultureInfo.InvariantCulture, " AND (AccessType = 'Public' OR Users Like '~{0}~')", userId));
            }

            if (searchQueryDetails.CategoryFilter.Count > 0)
            {
                string categoryList = string.Join(",", searchQueryDetails.CategoryFilter);
                queryCondition = queryCondition.Append(string.Format(CultureInfo.InvariantCulture, " AND (Category IN ({0}))", categoryList));
            }

            if (searchQueryDetails.ContentTypeFilter.Count > 0)
            {
                string contentTypeList = string.Join(",", searchQueryDetails.ContentTypeFilter);
                queryCondition = queryCondition.Append(string.Format(CultureInfo.InvariantCulture, " AND (ContentType IN ({0}))", contentTypeList));
            }

            return queryCondition.ToString();
        }

        /// <summary>
        /// Gets the ORDER BY clause for the select query while searching communities and contents.
        /// </summary>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Order by condition to be used in the query</returns>
        private static string GetSearchOrderBy(SearchQueryDetails searchQueryDetails)
        {
            // default order by is Rating.
            string orderBy = string.Empty;

            switch (searchQueryDetails.SortBy)
            {
                case SearchSortBy.Categories:
                    orderBy = " ORDER BY Category";
                    break;
                case SearchSortBy.DistributedBy:
                    orderBy = " ORDER BY DistributedBy";
                    break;
                case SearchSortBy.ContentType:
                    orderBy = " ORDER BY ContentType";
                    break;
                case SearchSortBy.None:
                case SearchSortBy.Rating:
                default:
                    orderBy = " ORDER BY Rating DESC, RatedPeople DESC";
                    break;
            }

            return orderBy;
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="ContentsViewRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Contents View repository having methods for retrieving content details
    /// from SQL Azure Layerscape database.
    /// </summary>
    public class ContentsViewRepository : RepositoryBase<ContentsView>, IContentsViewRepository
    {
        /// <summary>
        /// Initializes a new instance of the ContentsViewRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Instance of Layerscape db context</param>
        public ContentsViewRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Retrieves the multiple instances of contents for the given IDs.
        /// </summary>
        /// <param name="contentIDs">
        /// Content IDs.
        /// </param>
        /// <param name="communityID">Community Id</param>
        /// <returns>
        /// Collection of ContentsView.
        /// </returns>
        public IEnumerable<ContentsView> GetItems(IEnumerable<long> contentIDs, long communityID)
        {
            IEnumerable<ContentsView> result = null;

            // TODO: Since the data is populated through data generator tools, there are contents part of multiple communities.
            // To avoid showing invalid data, additional parameter and check is added which needs to be removed later.
            result = from contents in DbSet
                     where contentIDs.Contains(contents.ContentID) && contents.CommunityID == communityID
                     orderby contents.LastUpdatedDatetime descending
                     select contents;

            return result.ToList();
        }

        /// <summary>
        /// Gets the search result count for the given search text from contents.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <returns>Count of search result items</returns>
        public int SearchContentsCount(string searchText, long userId)
        {
            return GetItems(GetContentSearchCondition(searchText, userId), GetContentOrderByCondition(), true).Count();
        }

        /// <summary>
        /// Gets the search results for the given search text from contents.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped based on pagination</param>
        /// <param name="takeCount">Items to be taken based on pagination</param>
        /// <returns>Search result items</returns>
        public IEnumerable<ContentsView> SearchContents(string searchText, long userId, int skipCount, int takeCount)
        {
            return GetItems(GetContentSearchCondition(searchText, userId), GetContentOrderByCondition(), true, skipCount, takeCount);
        }

        /// <summary>
        /// Gets total consumed size for the user.
        /// </summary>
        /// <param name="userID">ID of the user.</param>
        /// <returns>Total consumed size.</returns>
        public decimal GetConsumedSize(long userID)
        {
            var consumedSize = Queryable.Where<Content>(this.EarthOnlineDbContext.Content, item => item.CreatedByID == userID && item.IsDeleted == false)
                .Sum(item => item.Size);

            return consumedSize.HasValue ? consumedSize.Value : 0;
        }

        /// <summary>
        /// Gets order by condition for content.
        /// </summary>
        /// <returns>Condition for ordering content.</returns>
        private static Func<ContentsView, object> GetContentOrderByCondition()
        {
            return (ContentsView c) => c.AverageRating;
        }

        /// <summary>
        /// Gets Search condition for content.
        /// </summary>
        /// <param name="searchText">searchText string</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Condition for Searching content.</returns>
        private Expression<Func<ContentsView, bool>> GetContentSearchCondition(string searchText, long userId)
        {
            searchText = searchText.ToLower(CultureInfo.CurrentCulture);
            return (ContentsView c) => (c.Title.ToLower().Contains(searchText) ||
                                            c.Description.ToLower().Contains(searchText) ||
                                            c.DistributedBy.ToLower().Contains(searchText) ||
                                            c.ProducedBy.ToLower().Contains(searchText) ||
                                            c.Citation.ToLower().Contains(searchText) ||
                                            c.Tags.ToLower().Contains(searchText)) &&
                                            (c.AccessType == Resources.Public || 
                                                Queryable.Where<User>(this.EarthOnlineDbContext.User, user => user.UserID == userId && user.UserTypeID == 1).FirstOrDefault() != null ||
                                                Queryable.Where<UserCommunities>(this.EarthOnlineDbContext.UserCommunities, uc => uc.UserID == userId && uc.CommunityId == c.CommunityID && uc.RoleID >= (int)UserRole.Reader).FirstOrDefault() != null ||
                                                c.CreatedByID == userId);
        }
    }
}
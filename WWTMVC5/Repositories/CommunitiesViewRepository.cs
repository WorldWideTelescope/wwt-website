//-----------------------------------------------------------------------
// <copyright file="CommunitiesViewRepository.cs" company="Microsoft Corporation">
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
    /// Class representing the Communities View repository having methods for retrieving community details
    /// from SQL Azure Layerscape database.
    /// </summary>
    public class CommunitiesViewRepository : RepositoryBase<CommunitiesView>, ICommunitiesViewRepository
    {
        /// <summary>
        /// Initializes a new instance of the CommunitiesViewRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Instance of Layerscape db context</param>
        public CommunitiesViewRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets the search result count for the given search text from communities
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <returns>Count of search result items</returns>
        public int SearchCommunitiesCount(string searchText, long userId)
        {
            return GetItems(GetCommunitiesSearchCondition(searchText, userId), GetCommunitiesOrderByCondition(), true).Count();
        }

        /// <summary>
        /// Gets the search results for the given search text from communities.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped based on pagination</param>
        /// <param name="takeCount">Items to be taken based on pagination</param>
        /// <returns>Search result items</returns>
        public IEnumerable<CommunitiesView> SearchCommunities(string searchText, long userId, int skipCount, int takeCount)
        {
            return GetItems(GetCommunitiesSearchCondition(searchText, userId), GetCommunitiesOrderByCondition(), true, skipCount, takeCount);
        }

        /// <summary>
        /// Gets order by condition for communities.
        /// </summary>
        /// <returns>Condition for ordering communities.</returns>
        private static Func<CommunitiesView, object> GetCommunitiesOrderByCondition()
        {
            return (CommunitiesView c) => c.AverageRating;
        }

        /// <summary>
        /// Gets Search condition for communities.
        /// </summary>
        /// <param name="searchText">searchText string</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Condition for Searching communities.</returns>
        private Expression<Func<CommunitiesView, bool>> GetCommunitiesSearchCondition(string searchText, long userId)
        {
            searchText = searchText.ToLower(CultureInfo.CurrentCulture);
            return (CommunitiesView c) => c.CommunityTypeID == (int)CommunityTypes.Community &&
                                            (c.CommunityName.ToLower().Contains(searchText) ||
                                                c.Description.ToLower().Contains(searchText) ||
                                                c.DistributedBy.ToLower().Contains(searchText) ||
                                                c.ProducedBy.ToLower().Contains(searchText) ||
                                                c.Tags.ToLower().Contains(searchText)) &&
                                            (c.AccessType == Resources.Public || 
                                                (Queryable.Where<User>(this.EarthOnlineDbContext.User, user => user.UserID == userId && user.UserTypeID == 1).FirstOrDefault() != null ||
                                                Queryable.Where<UserCommunities>(this.EarthOnlineDbContext.UserCommunities, uc => uc.UserID == userId && uc.CommunityId == c.CommunityID && uc.RoleID >= (int)UserRole.Reader).FirstOrDefault() != null));
        }
    }
}
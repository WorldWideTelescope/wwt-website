//-----------------------------------------------------------------------
// <copyright file="IUserRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the User repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IUserRepository : IRepositoryBase<User>
    {
        /// <summary>
        /// Gets the users roles on the given community. If community is not passed, this method checks only for the site admin role.
        /// </summary>
        /// <param name="userId">User for whom role to be found</param>
        /// <param name="communityId">Community Id on which user role to be found</param>
        /// <returns>Role of user on the given community, for site admin it will be always Owner for all the communities</returns>
        UserRole GetUserRole(long userId, long? communityId);

        /// <summary>
        /// Gets the communities of the given user to which he is having the same or higher role specified.
        /// </summary>
        /// <param name="userId">User for whom communities to be fetched</param>
        /// <param name="userRole">Role (equal or higher) which the user should in the community</param>
        /// <param name="onlyPublic">Consider the private communities also or only public</param>
        /// <returns>List of community Ids</returns>
        IEnumerable<long> GetUserCommunitiesForRole(long userId, UserRole userRole, bool onlyPublic);

        /// <summary>
        /// Checks whether any user requests are pending for approval for the given user.
        /// </summary>
        /// <param name="userId">User for whom pending requests to be checked</param>
        /// <param name="communityId">Community on which user requests to be checked</param>
        /// <returns>True if requests are pending, false otherwise</returns>
        bool PendingPermissionRequests(long userId, long communityId);

        /// <summary>
        /// Retrieves the latest profile IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestProfileIDs(int count);
        
        /// <summary>
        /// Check if the user is site Admin or not.
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        /// <returns>True if user is site admin;Otherwise false.</returns>
        bool IsSiteAdmin(long userId);
    }
}
//-----------------------------------------------------------------------
// <copyright file="UserRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the user repository having methods for retrieving user details
    /// from SQL Azure Layerscape database.
    /// </summary>
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Instance of Layerscape db context</param>
        public UserRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets the users roles on the given community. If community is not passed, this method checks only for the site admin role.
        /// </summary>
        /// <param name="userId">User for whom role to be found</param>
        /// <param name="communityId">Community Id on which user role to be found</param>
        /// <returns>Role of user on the given community, for site admin it will be always Owner for all the communities</returns>
        public UserRole GetUserRole(long userId, long? communityId)
        {
            UserRole userRole = UserRole.Visitor;
            UserCommunities userCommunityRole = null;

            // 1. Check if the user is site administrator.
            // 2. Get the user's role for the given community.
            User user = Queryable.Where<User>(this.EarthOnlineDbContext.User, u => u.UserID == userId).Include<User, ICollection<UserCommunities>>(u => u.UserCommunities).FirstOrDefault();
            
            if (user != null)
            {
                if (user.UserTypeID == 1)
                {
                    userRole = UserRole.SiteAdmin;
                }
                else if (communityId.HasValue &&
                            (userCommunityRole = Enumerable.Where<UserCommunities>(user.UserCommunities, uc => uc.CommunityId == communityId).FirstOrDefault()) != null)
                {
                    userRole = (UserRole)userCommunityRole.RoleID;

                    // In case of moderator role, need to check if the permissions are inherited.
                    if (userRole == UserRole.Moderator && userCommunityRole.IsInherited)
                    {
                        userRole = UserRole.ModeratorInheritted;
                    }
                }
            }

            return userRole;
        }

        /// <summary>
        /// Gets the communities of the given user to which he is having the same or higher role specified.
        /// </summary>
        /// <param name="userId">User for whom communities to be fetched</param>
        /// <param name="userRole">Role (equal or higher) which the user should in the community</param>
        /// <param name="onlyPublic">Consider the private communities also or only public</param>
        /// <returns>List of community Ids</returns>
        public IEnumerable<long> GetUserCommunitiesForRole(long userId, UserRole userRole, bool onlyPublic)
        {
            // Get the communities to which user is having given role or more.
            var userCommunityIds = Queryable.Select<UserCommunities, long>(Queryable.Where<UserCommunities>(this.EarthOnlineDbContext.UserCommunities, communityRole => communityRole.UserID == userId && communityRole.RoleID >= (int)userRole), communityRole => communityRole.CommunityId);

            // Get the communities which are not deleted.
            var result = Queryable.Select<Community, long>(Queryable.OrderByDescending<Community, DateTime?>(Queryable.Where<Community>(this.EarthOnlineDbContext.Community, community => userCommunityIds.Contains(community.CommunityID) && 
                !(bool)community.IsDeleted && 
                community.CommunityTypeID != (int)CommunityTypes.User &&
                (onlyPublic ? community.AccessTypeID == (int)AccessType.Public : !onlyPublic)), community => community.ModifiedDatetime), community => community.CommunityID);

            return result.ToList();
        }

        /// <summary>
        /// Checks whether any user requests are pending for approval for the given user.
        /// </summary>
        /// <param name="userId">User for whom pending requests to be checked</param>
        /// <param name="communityId">Community on which user requests to be checked</param>
        /// <returns>True if requests are pending, false otherwise</returns>
        public bool PendingPermissionRequests(long userId, long communityId)
        {
            bool pendingPermissionRequests = false;

            if (Queryable.Where<PermissionRequest>(this.EarthOnlineDbContext.PermissionRequest, user => user.UserID == userId && user.CommunityID == communityId && user.Approved == null).FirstOrDefault() != null)
            {
                pendingPermissionRequests = true;
            }

            return pendingPermissionRequests;
        }

        /// <summary>
        /// Retrieves the latest profile IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        public IEnumerable<long> GetLatestProfileIDs(int count)
        {
            // Get the profiles which are not deleted.
            var result = Queryable.Select<User, long>(Queryable.OrderByDescending<User, long>(Queryable.Where<User>(this.EarthOnlineDbContext.User, user => !(bool)user.IsDeleted), user => user.UserID), user => user.UserID)
                                .Take(count);

            return result.ToList();
        }

        /// <summary>
        /// Check if the user is site Admin or not.
        /// </summary>
        /// <param name="userID">Id of the user.</param>
        /// <returns>True if user is site admin;Otherwise false.</returns>
        public bool IsSiteAdmin(long userID)
        {
            bool isSiteAdmin = false;

            var userInstance = Queryable.Where<User>(this.EarthOnlineDbContext.User, user => user.UserID == userID).FirstOrDefault();
            if (userInstance != null)
            {
                isSiteAdmin = userInstance.UserTypeID == (int)UserTypes.SiteAdmin;
            }

            return isSiteAdmin;
        }
    }
}
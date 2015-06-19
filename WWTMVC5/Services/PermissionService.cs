//-----------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Community service having methods for retrieving community
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class PermissionService
    {
        #region Private Variables

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunityRepository _communityRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository _userRepository;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PermissionService class.
        /// </summary>
        /// <param name="communityRepository">Instance of Community repository</param>
        /// <param name="userRepository">Instance of User repository</param>
        public PermissionService(
            ICommunityRepository communityRepository,
            IUserRepository userRepository)
        {
            this._communityRepository = communityRepository;
            this._userRepository = userRepository;
        }

        #endregion Constructor

        #region Private Methods

        /// <summary>
        /// Check whether the user can read the community.
        /// </summary>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to read the community; Otherwise False.</returns>
        protected static bool CanReadCommunity(UserRole userRole)
        {
            var canRead = false;
            if (userRole >= UserRole.Visitor)
            {
                canRead = true;
            }

            return canRead;
        }

        /// <summary>
        /// Check whether the user can create content.
        /// </summary>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to create content; Otherwise False.</returns>
        protected static bool CanCreateContent(UserRole userRole)
        {
            var canCreate = false;

            // Owners, moderators and contributors can add content
            canCreate = userRole >= UserRole.Contributor;

            return canCreate;
        }

        /// <summary>
        /// Check whether the user can edit/delete the content.
        /// </summary>
        /// <param name="content">Content instance</param>
        /// <param name="userId">User Identification</param>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to edit/delete  the content; Otherwise False.</returns>
        protected static bool CanEditDeleteContent(Content content, long userId, UserRole userRole)
        {
            var canDelete = false;
            if (content != null)
            {
                if (userRole == UserRole.Contributor && content.CreatedByID == userId)
                {
                    // Contributors can edit/delete only content that they have added, they cannot modify/delete content added by others
                    canDelete = true;
                }
                else if (userRole >= UserRole.Moderator)
                {
                    // Owners and moderators can edit/remove content added by anyone else
                    canDelete = true;
                }
            }

            return canDelete;
        }

        /// <summary>
        /// Check whether the user can read the content.
        /// </summary>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to read the content; Otherwise False.</returns>
        protected static bool CanReadContent(UserRole userRole)
        {
            var canRead = false;
            if (userRole >= UserRole.Visitor)
            {
                canRead = true;
            }

            return canRead;
        }

        /// <summary>
        /// Check whether the user can create community.
        /// Create communities
        ///     a. At the root level, whichever user that creates a community becomes the owner of it. 
        ///     b. Under an existing community (folders and sub-communities are used interchangeably), 
        ///         only owners and moderators can create sub communities
        /// </summary>
        /// <param name="parentCommunityId">Parent Community ID</param>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to create community; Otherwise False.</returns>
        protected static bool CanCreateCommunity(long? parentCommunityId, UserRole userRole)
        {
            // a.   At the root level, whichever user that creates a community becomes the owner of it
            var canCreate = true;

            if (parentCommunityId.HasValue && parentCommunityId.Value > 0)
            {
                // b.   Under an existing community (folders and sub-communities are used interchangeably), 
                //      only owners and moderators can create sub communities
                canCreate = userRole >= UserRole.Moderator;
            }

            return canCreate;
        }

        /// <summary>
        /// Gets the role of the user on the given Community.
        /// </summary>
        /// <param name="communityId">Community Id on which user role has to be found</param>
        /// <param name="userId">Current user id</param>
        /// <returns>UserRole on the Community</returns>
        protected UserRole GetCommunityUserRole(long communityId, long? userId)
        {
            var userRole = UserRole.Visitor;

            if (userId.HasValue && userId.Value > 0)
            {
                userRole = _userRepository.GetUserRole(userId.Value, communityId);
            }

            // In case if Private content, only site administrators or users who are owners/moderators/contributors/readers can access them.
            if (userRole < UserRole.Reader && AccessType.Private.ToString() == _communityRepository.GetCommunityAccessType(communityId))
            {
                return UserRole.None;
            }

            return userRole;
        }

        /// <summary>
        /// Check whether the user can edit/delete the community.
        /// </summary>
        /// <param name="community">Community instance</param>
        /// <param name="userId">User Identification</param>
        /// <param name="userRole">Role of the User.</param>
        /// <returns>True if the user has permission to edit/delete the community; Otherwise False.</returns>
        protected bool CanEditDeleteCommunity(Community community, long userId, UserRole userRole)
        {
            var canEditDelete = false;

            if (community != null)
            {
                if (community.CommunityRelation1.Count == 0)
                {
                    // a.   At the root level only an owner or Site Administrator can modify the community
                    canEditDelete = userRole >= UserRole.Owner;
                }
                else
                {
                    var parentCommunityId = Enumerable.ElementAt(community.CommunityRelation1, 0).ParentCommunityID;

                    if (userRole >= UserRole.ModeratorInheritted)
                    {
                        // Inherited Moderator can edit/delete the current community.
                        canEditDelete = true;
                    }
                    else if (userRole == UserRole.Contributor && community.CreatedByID == userId)
                    {
                        // Contributors can edit/delete only community that they have added, they cannot modify/delete community added by others
                        canEditDelete = true;
                    }
                    else if (GetCommunityUserRole(parentCommunityId, userId) >= UserRole.Moderator)
                    {
                        // b.   A moderator can edit/delete any communities/content including those created by others (even the owner) 
                        //      as long as he is the moderator of the parent of the community/content
                        canEditDelete = true;
                    }
                }
            }

            return canEditDelete;
        }

        #endregion Private Methods
    }
}

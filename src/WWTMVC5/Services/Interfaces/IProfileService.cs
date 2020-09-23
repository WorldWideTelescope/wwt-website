//-----------------------------------------------------------------------
// <copyright file="IProfileService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the user profile.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Gets the user profile from the puid
        /// </summary>
        /// <param name="puid">Live user PUID</param>
        /// <returns>ProfileDetails object</returns>
        Task<ProfileDetails> GetProfileAsync(string puid);

        ProfileDetails GetProfile(string puid);
        
        /// <summary>
        /// Gets the user profile from USER ID
        /// </summary>
        /// <param name="userId">User profile ID</param>
        /// <returns>ProfileDetails object</returns>
        ProfileDetails GetProfile(long userId);

        /// <summary>
        /// Return profile details for the list of user ID
        /// </summary>
        /// <param name="users">User profile IDs</param>
        /// <returns>List of ProfileDetails object</returns>
        Task<IEnumerable<ProfileDetails>> GetProfilesAsync(IEnumerable<long> users);
        IEnumerable<ProfileDetails> GetProfiles(IEnumerable<long> users);

        /// <summary>
        /// Created the user profile
        /// </summary>
        /// <param name="profileDetails">ProfileDetails object</param>
        /// <returns>Profile ID</returns>
        Task<long> CreateProfileAsync(ProfileDetails profileDetails);
        long CreateProfile(ProfileDetails profileDetails);
        
        /// <summary>
        /// Updates the profile details to database.
        /// </summary>
        /// <param name="profile">Profile information.</param>
        /// <returns>True if the profile has been updated successfully; Otherwise false.</returns>
        Task<bool> UpdateProfileAsync(ProfileDetails profile);
        bool UpdateProfile(ProfileDetails profile);
        
        /// <summary>
        /// Gets the communities from the Layerscape database for the given owner.
        /// </summary>
        /// <param name="userId">Communities Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all communities</returns>
        Task<IEnumerable<CommunityDetails>> GetCommunitiesAsync(long userId, PageDetails pageDetails, bool onlyPublic);
        IEnumerable<CommunityDetails> GetCommunities(long userId, PageDetails pageDetails, bool onlyPublic);
        
        /// <summary>
        /// Gets the content from the Layerscape database for the given user.
        /// </summary>
        /// <param name="userId">Contents Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all contents</returns>
        Task<IEnumerable<ContentDetails>> GetContentsAsync(long userId, PageDetails pageDetails, bool onlyPublic);
        IEnumerable<ContentDetails> GetContents(long userId, PageDetails pageDetails, bool onlyPublic);

        /// <summary>
        /// Gets the total number of communities for the given user.
        /// </summary>
        /// <param name="userId">Communities Owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of communities.</returns>
        Task<int> GetCommunitiesCountAsync(long userId, bool onlyPublic);
        int GetCommunitiesCount(long userId, bool onlyPublic);

        /// <summary>
        /// Gets the total number of contents for the given user.
        /// </summary>
        /// <param name="userId">Contents Owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of contents.</returns>
        Task<int> GetContentsCountAsync(long userId, bool onlyPublic);
        int GetContentsCount(long userId, bool onlyPublic);

        /// <summary>
        /// Gets the user permissions for the given community and for the given page. User should have at least
        /// contributor permission on the community to get user permissions.
        /// </summary>
        /// <param name="userId">User who is reading the permissions</param>
        /// <param name="communityId">Community for which permissions are fetched</param>
        /// <param name="pageDetails">Page for which permissions are fetched</param>
        /// <returns>List of permissions/user roles</returns>
        Task<PermissionDetails> GetUserPemissions(long userId, long communityId, PageDetails pageDetails);

        /// <summary>
        /// Gets the user requests for the given community and for the given page. User should have moderator
        /// or owner/site admin permission on the community to get user request.
        /// </summary>
        /// <param name="userId">User who is reading the requests</param>
        /// <param name="communityId">Community for which requests are fetched</param>
        /// <param name="pageDetails">Page for which requests are fetched</param>
        /// <returns>List of user role requests</returns>
        Task<PermissionDetails> GetUserPemissionRequests(long userId, long? communityId, PageDetails pageDetails);

        /// <summary>
        /// Gets the invite requests which are already sent for the given community and which are not yet used.
        /// </summary>
        /// <param name="userId">User who is reading the invite requests</param>
        /// <param name="communityId">Community for which invite requests are fetched</param>
        /// <param name="pageDetails">Page for which invite requests are fetched</param>
        /// <returns>List of open invite requests for the community</returns>
        Task<IEnumerable<InviteRequestItem>> GetInviteRequests(long userId, long communityId, PageDetails pageDetails);

        /// <summary>
        /// Adds the Join request of the user to a community for the given role.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <returns>True if the request is added, false otherwise</returns>
        OperationStatus JoinCommunity(PermissionItem permissionItem);

        /// <summary>
        /// Joins the current user to community for which the invite request token was generated.
        /// </summary>
        /// <param name="userId">User who is making the join request</param>
        /// <param name="inviteRequestToken">Token to be used for joining the community</param>
        /// <returns>Status of the operation. Success, if succeeded, failure message and exception details in case of exception.</returns>
        Task<OperationStatus> JoinCommunity(long userId, Guid inviteRequestToken);

        /// <summary>
        /// Approves or declines a permission request of a user for a community.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedById">User who is updating the permission request</param>
        /// <returns>True if the request is updated, false otherwise</returns>
        OperationStatus UpdateUserPermissionRequest(PermissionItem permissionItem, long updatedById);

        /// <summary>
        /// Updates (changing the role or deleting the role) the permission request for a user for a community.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedById">User who is updating the permission request</param>
        /// <returns>True if the permission is updated, false otherwise</returns>
        OperationStatus UpdateUserRoles(PermissionItem permissionItem, long updatedById);

        /// <summary>
        /// Retrieves the latest profile IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestProfileIDs(int count);

        /// <summary>
        /// Checks if the user is Site Admin
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>True if user is site admin;Otherwise false.</returns>
        OperationStatus IsSiteAdmin(long userId);

        /// <summary>
        /// Removes the specified invite request.
        /// </summary>
        /// <param name="userId">User who is removing the invite request</param>
        /// <param name="inviteRequestId">Invite request to be removed</param>
        /// <returns>True if the invite request is removed, false otherwise</returns>
        Task<OperationStatus> RemoveInviteRequest(long userId, int inviteRequestId);

        /// <summary>
        /// This function is used to get all profiles in the database excluding the current user.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="userId">ID the of the current user.</param>
        /// <returns>List of all profile in database.</returns>
        Task<IEnumerable<ProfileDetails>> GetAllProfiles(long userId);

        /// <summary>
        /// This function is used to promote the users with the ID specified as Site administrators.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="adminUsers">Admin user list who has to be promoted to site administrators.</param>
        /// <param name="updatedById">ID the of the current user.</param>
        /// <returns>True if the Users has been promoted.</returns>
        Task<OperationStatus> PromoteAsSiteAdmin(IEnumerable<long> adminUsers, long updatedById);
        
        
    }
}

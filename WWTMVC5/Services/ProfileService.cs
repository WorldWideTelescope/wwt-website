﻿//-----------------------------------------------------------------------
// <copyright file="ProfileService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Profile service class for handling profile related functionality
    /// </summary>
    public class ProfileService : IProfileService
    {
        #region Members

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository userRepository;

        /// <summary>
        /// Instance of Contents repository
        /// </summary>
        private IContentsViewRepository contentsViewRepository;

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunitiesViewRepository communitiesViewRepository;

        /// <summary>
        /// Instance of UserCommunities repository
        /// </summary>
        private IUserCommunitiesRepository userCommunitiesRepository;

        /// <summary>
        /// Instance of Permission repository
        /// </summary>
        private IRepositoryBase<PermissionRequest> permissionRequestRepository;

        /// <summary>
        /// Instance of Blob data repository
        /// </summary>
        private IBlobDataRepository blobDataRepository;

        /// <summary>
        /// Instance of InviteRequestsView repository
        /// </summary>
        private IRepositoryBase<InviteRequestsView> inviteRequestsViewRepository;

        /// <summary>
        /// Instance of InviteRequest repository
        /// </summary>
        private IRepositoryBase<InviteRequest> inviteRequestRepository;

        /// <summary>
        /// Instance of UserType repository
        /// </summary>
        private IRepositoryBase<UserType> userTypeRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ProfileService class.
        /// </summary>
        /// <param name="userRepository">Instance of user repository</param>
        /// <param name="contentsViewRepository">Instance of content repository</param>
        /// <param name="communitiesViewRepository">Instance of CommunitiesView repository</param>
        /// <param name="userCommunitiesRepository">Instance of UserCommunities repository</param>
        /// <param name="permissionRequestRepository">Instance of PermissionRequest repository</param>
        /// <param name="blobDataRepository">Instance of Blob data repository</param>
        /// <param name="inviteRequestsViewRepository">Instance of InviteRequestsView repository</param>
        /// <param name="inviteRequestRepository">Instance of InviteRequest repository</param>
        public ProfileService(
            IUserRepository userRepository,
            IContentsViewRepository contentsViewRepository,
            ICommunitiesViewRepository communitiesViewRepository,
            IUserCommunitiesRepository userCommunitiesRepository,
            IRepositoryBase<PermissionRequest> permissionRequestRepository,
            IBlobDataRepository blobDataRepository,
            IRepositoryBase<InviteRequestsView> inviteRequestsViewRepository,
            IRepositoryBase<InviteRequest> inviteRequestRepository,
            IRepositoryBase<UserType> userTypeRepository)
        {
            this.userRepository = userRepository;
            this.contentsViewRepository = contentsViewRepository;
            this.communitiesViewRepository = communitiesViewRepository;
            this.userCommunitiesRepository = userCommunitiesRepository;
            this.permissionRequestRepository = permissionRequestRepository;
            this.blobDataRepository = blobDataRepository;
            this.inviteRequestsViewRepository = inviteRequestsViewRepository;
            this.inviteRequestRepository = inviteRequestRepository;
            this.userTypeRepository = userTypeRepository;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Gets the user profile from the puid
        /// </summary>
        /// <param name="puid">Live user PUID</param>
        /// <returns>ProfileDetails object</returns>
        public ProfileDetails GetProfile(string puid)
        {
            ProfileDetails profileDetails = null;
            var userDetails = this.userRepository.GetItem(user => user.LiveID == puid);

            // Check if the user details is present.
            // DO NOT use CheckNotNull since GetProfile should not throw exception.
            if (userDetails != null)
            {
                profileDetails = new ProfileDetails();
                Mapper.Map(userDetails, profileDetails);

                // Set the consumed size
                profileDetails.ConsumedSize = this.contentsViewRepository.GetConsumedSize(userDetails.UserID);
            }

            return profileDetails;
        }

        /// <summary>
        /// Gets the user profile from USER ID
        /// </summary>
        /// <param name="userID">User profile ID</param>
        /// <returns>ProfileDetails object</returns>
        public ProfileDetails GetProfile(long userID)
        {
            var profileDetails = new ProfileDetails();
            var userDetails = this.userRepository.GetItem(user => user.UserID == userID, "UserType");

            // Check if the user details is present.
            this.CheckNotNull(() => new { userDetails });

            Mapper.Map(userDetails, profileDetails);

            // Set the consumed size
            profileDetails.ConsumedSize = this.contentsViewRepository.GetConsumedSize(userID);

            return profileDetails;
        }

        /// <summary>
        /// Gets the user profile details for the list of user ID
        /// </summary>
        /// <param name="users">User profile ID list</param>
        /// <returns>ProfileDetails objects list</returns>
        public IEnumerable<ProfileDetails> GetProfiles(IEnumerable<long> users)
        {
            var profileDetails = new List<ProfileDetails>();
            Expression<Func<User, bool>> condition = (user) => (users.Contains(user.UserID));
            var userDetails = this.userRepository.GetItems(condition, null, false);

            if (userDetails != null)
            {
                foreach (var userDetail in userDetails)
                {
                    var profileDetail = new ProfileDetails();
                    Mapper.Map(userDetail, profileDetail);
                    profileDetails.Add(profileDetail);  
                }
            }

            return profileDetails;
        }

        /// <summary>
        /// Updates the profile details to database.
        /// </summary>
        /// <param name="profile">Profile information.</param>
        /// <returns>True if the profile has been updated successfully; Otherwise false.</returns>
        public bool UpdateProfile(ProfileDetails profile)
        {
            bool status = false;
            var userDetails = this.userRepository.GetItem(user => user.UserID == profile.ID);
            if (userDetails != null)
            {
                try
                {
                    // Only in case if new Picture is uploaded, move it from temporary container.
                    if (userDetails.PictureID != profile.PictureID)
                    {
                        profile.PictureID = MoveThumbnail(profile.PictureID);
                    }

                    // Set About me and Affiliation details
                    userDetails.Affiliation = profile.Affiliation;
                    userDetails.AboutMe = profile.AboutMe;
                    userDetails.FirstName = profile.FirstName;
                    userDetails.LastName = profile.LastName;
                    userDetails.IsSubscribed = profile.IsSubscribed;
                    userDetails.PictureID = profile.PictureID;
                    userDetails.LastLoginDatetime = profile.LastLogOnDatetime;

                    // Add the user to the repository
                    this.userRepository.Update(userDetails);

                    // Save all the changes made.
                    this.userRepository.SaveChanges();

                    status = true;
                }
                catch (Exception)
                {
                    // Delete Uploaded Thumbnail
                    DeleteThumbnail(profile.PictureID);
                    throw;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the communities from the Layerscape database for the given owner.
        /// </summary>
        /// <param name="userID">Communities Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all communities</returns>
        public IEnumerable<CommunityDetails> GetCommunities(long userID, PageDetails pageDetails, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID, pageDetails });

            IList<CommunityDetails> userCommunities = new List<CommunityDetails>();

            Func<CommunitiesView, object> orderBy = (CommunitiesView c) => c.LastUpdatedDatetime;

            // Get all the community ids to which user is given role of contributor or higher.
            IEnumerable<long> userCommunityIds = this.userRepository.GetUserCommunitiesForRole(userID, UserRole.Contributor, onlyPublic);

            // Get only the communities for the current page.
            userCommunityIds = userCommunityIds.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);

            Expression<Func<CommunitiesView, bool>> condition = (CommunitiesView c) => userCommunityIds.Contains(c.CommunityID);

            foreach (var community in this.communitiesViewRepository.GetItems(condition, orderBy, true))
            {
                CommunityDetails communityDetails = null;
                if (onlyPublic)
                {
                    // In case of only public, user is looking at somebody else profile and so just send the user role as Visitor.
                    communityDetails = new CommunityDetails(Permission.Visitor);
                }
                else
                {
                    UserRole userRole = this.userRepository.GetUserRole(userID, community.CommunityID);
                    communityDetails = new CommunityDetails(userRole.GetPermission());
                }

                Mapper.Map(community, communityDetails);
                userCommunities.Add(communityDetails);
            }

            return userCommunities;
        }

        /// <summary>
        /// Gets the content from the Layerscape database for the given user.
        /// </summary>
        /// <param name="userID">Contents Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all contents</returns>
        public IEnumerable<ContentDetails> GetContents(long userID, PageDetails pageDetails, bool onlyPublic)
        {
            IList<ContentDetails> contents = new List<ContentDetails>();
            Func<ContentsView, object> orderBy = null;
            Expression<Func<ContentsView, bool>> condition = null;

            this.CheckNotNull(() => new { userID, pageDetails });

            orderBy = (ContentsView c) => c.LastUpdatedDatetime;

            if (onlyPublic)
            {
                string accessType = AccessType.Public.ToString();
                condition = (ContentsView c) => c.CreatedByID == userID && c.AccessType == accessType;
            }
            else
            {
                condition = (ContentsView c) => c.CreatedByID == userID;
            }

            foreach (var content in this.contentsViewRepository.GetItems(
                                                condition, orderBy, true, ((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage), pageDetails.ItemsPerPage))
            {
                ContentDetails contentDetails = null;
                if (onlyPublic)
                {
                    contentDetails = new ContentDetails(Permission.Visitor);
                }
                else
                {
                    contentDetails = new ContentDetails(Permission.Contributor);
                }

                Mapper.Map(content, contentDetails);
                contents.Add(contentDetails);
            }

            return contents;
        }

        /// <summary>
        /// Gets the total number of communities for the given user.
        /// </summary>
        /// <param name="userID">Communities owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of communities.</returns>
        public int GetCommunitiesCount(long userID, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID });

            return this.userRepository.GetUserCommunitiesForRole(userID, UserRole.Contributor, onlyPublic).Count();
        }

        /// <summary>
        /// Gets the total number of contents for the given user.
        /// </summary>
        /// <param name="userID">Contents Owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of contents.</returns>
        public int GetContentsCount(long userID, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID });

            Expression<Func<ContentsView, bool>> condition = null;
            if (onlyPublic)
            {
                string accessType = AccessType.Public.ToString();
                condition = (ContentsView c) => c.CreatedByID == userID && c.AccessType == accessType;
            }
            else
            {
                condition = (ContentsView c) => c.CreatedByID == userID;
            }

            return this.contentsViewRepository.GetItemsCount(condition);
        }

        /// <summary>
        /// Created the user profile
        /// </summary>
        /// <param name="profileDetails">ProfileDetails object</param>
        /// <returns>Profile ID</returns>
        public long CreateProfile(ProfileDetails profileDetails)
        {
            // Make sure communityDetails is not null
            this.CheckNotNull(() => new { profileDetails });

            // Check if the user already exists in the Layerscape database.
            User existingUser = this.userRepository.GetItem(u => u.LiveID == profileDetails.PUID);

            if (existingUser != null)
            {
                return existingUser.UserID;
            }
            else
            {
                // 1. Add Community details to the community object.
                User user = new User();
                Mapper.Map(profileDetails, user);

                user.JoinedDateTime = DateTime.UtcNow;
                user.LastLoginDatetime = DateTime.UtcNow;

                // While creating the user, IsDeleted to be false always.
                user.IsDeleted = false;

                // Add the user to the repository
                this.userRepository.Add(user);

                // Save all the changes made.
                this.userRepository.SaveChanges();

                return user.UserID;
            }
        }

        /// <summary>
        /// Gets the user permissions for the given community and for the given page. User should have at least
        /// contributor permission on the community to get user permissions.
        /// </summary>
        /// <param name="userID">User who is reading the permissions</param>
        /// <param name="communityID">Community for which permissions are fetched</param>
        /// <param name="pageDetails">Page for which permissions are fetched</param>
        /// <returns>List of permissions/user roles</returns>
        public PermissionDetails GetUserPemissions(long userID, long communityID, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            Expression<Func<UserCommunities, bool>> condition = (UserCommunities c) => c.CommunityId == communityID;
            Func<UserCommunities, object> orderBy = (UserCommunities c) => c.RoleID;

            // Gets the total items satisfying the condition
            pageDetails.TotalCount = this.userCommunitiesRepository.GetItemsCount(condition);
            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // TODO: Passing the condition in a variable doesn't add the WHERE clause in SQL server. Need to work on this later.
            IEnumerable<UserCommunities> items = null;

            items = this.userCommunitiesRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            PermissionDetails permissionDetails = new PermissionDetails();

            if (items != null && items.Count() > 0)
            {
                UserRole userRole = this.userRepository.GetUserRole(userID, communityID);

                // User has to be at least contributor to know the permission details of the community.
                if (userRole >= UserRole.Contributor)
                {
                    permissionDetails.CurrentUserPermission = userRole.GetPermission();

                    foreach (var item in items)
                    {
                        PermissionItem permissionItem = new PermissionItem();
                        Mapper.Map(item, permissionItem);
                        permissionItem.CurrentUserRole = userRole;
                        permissionDetails.PermissionItemList.Add(permissionItem);
                    }
                }
                else
                {
                    // If user is not having contributor or higher role, he will get item not found or don't have permission exception page.
                    permissionDetails = null;
                }
            }

            return permissionDetails;
        }

        /// <summary>
        /// Gets the user requests for the given community and for the given page. User should have moderator
        /// or owner/site admin permission on the community to get user request.
        /// </summary>
        /// <param name="userID">User who is reading the requests</param>
        /// <param name="communityID">Community for which requests are fetched</param>
        /// <param name="pageDetails">Page for which requests are fetched</param>
        /// <returns>List of user role requests</returns>
        public PermissionDetails GetUserPemissionRequests(long userID, long? communityID, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            // Condition to get all the pending requests irrespective of community.
            Expression<Func<PermissionRequest, bool>> condition = (PermissionRequest pr) => pr.Approved == null;
            Func<PermissionRequest, object> orderBy = (PermissionRequest c) => c.RoleID;

            if (communityID.HasValue)
            {
                // If community is specified, get all the pending requests of the specified community.
                condition = (PermissionRequest pr) => pr.Approved == null && pr.CommunityID == communityID.Value;
            }
            else
            {
                // If no community id is specified, get all the community ids to which user is given role of moderator or 
                // higher and get their pending requests.
                IEnumerable<long> userCommunityIds = this.userRepository.GetUserCommunitiesForRole(userID, UserRole.Moderator, false);

                condition = (PermissionRequest pr) => pr.Approved == null && userCommunityIds.Contains(pr.CommunityID);
            }

            // Gets the total items satisfying the condition
            pageDetails.TotalCount = this.permissionRequestRepository.GetItemsCount(condition);
            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            PermissionDetails permissionDetails = new PermissionDetails();

            foreach (var item in this.permissionRequestRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage))
            {
                UserRole userRole = this.userRepository.GetUserRole(userID, item.CommunityID);

                // 1. User has to be at least Moderator to know the permission request details of the community.
                // 2. In case of profile page, user might be moderator for few communities and not for others. So, need to send only the requests
                //    of community to which user is moderator or higher.
                if (userRole >= UserRole.Moderator)
                {
                    PermissionItem permissionItem = new PermissionItem();
                    Mapper.Map(item, permissionItem);
                    permissionItem.CurrentUserRole = userRole;
                    permissionDetails.PermissionItemList.Add(permissionItem);
                    permissionDetails.CurrentUserPermission = userRole.GetPermission();
                }
                else if (communityID.HasValue)
                {
                    // If user is not having contributor or higher role, he will get item not found or don't have permission exception page.
                    // This message to be shown only in case of permissions page not for profile page.
                    permissionDetails = null;
                }
            }

            return permissionDetails;
        }

        /// <summary>
        /// Gets the invite requests which are already sent for the given community and which are not yet used.
        /// </summary>
        /// <param name="userID">User who is reading the invite requests</param>
        /// <param name="communityID">Community for which invite requests are fetched</param>
        /// <param name="pageDetails">Page for which invite requests are fetched</param>
        /// <returns>List of open invite requests for the community</returns>
        public IEnumerable<InviteRequestItem> GetInviteRequests(long userID, long communityID, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            IList<InviteRequestItem> inviteRequestItemList = new List<InviteRequestItem>();

            UserRole userRole = this.userRepository.GetUserRole(userID, communityID);
            if (userRole >= UserRole.Moderator)
            {
                // Condition to get all the pending requests irrespective of community.
                Expression<Func<InviteRequestsView, bool>> condition = (InviteRequestsView invite) => invite.Used == false && invite.CommunityID == communityID;
                Func<InviteRequestsView, object> orderBy = (InviteRequestsView invite) => invite.InvitedDate;

                // Gets the total items satisfying the condition
                pageDetails.TotalCount = this.inviteRequestsViewRepository.GetItemsCount(condition);
                pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

                foreach (var item in this.inviteRequestsViewRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage))
                {
                    InviteRequestItem inviteRequestItem = new InviteRequestItem();
                    Mapper.Map(item, inviteRequestItem);

                    inviteRequestItemList.Add(inviteRequestItem);
                }
            }

            return inviteRequestItemList;
        }

        /// <summary>
        /// Removes the specified invite request.
        /// </summary>
        /// <param name="userID">User who is removing the invite request</param>
        /// <param name="inviteRequestID">Invite request to be removed</param>
        /// <returns>True if the invite request is removed, false otherwise</returns>
        public OperationStatus RemoveInviteRequest(long userID, int inviteRequestID)
        {
            OperationStatus operationStatus = new OperationStatus();

            try
            {
                // Find the invite request entity in database.
                InviteRequest inviteRequest = this.inviteRequestRepository.GetItem(ir => ir.InviteRequestID == inviteRequestID, "InviteRequestContent");

                // Check invite request is not null
                this.CheckNotNull(() => new { inviteRequest });

                UserRole userRole = this.userRepository.GetUserRole(userID, inviteRequest.InviteRequestContent.CommunityID);
                if (userRole >= UserRole.Moderator)
                {
                    inviteRequest.IsDeleted = true;
                    inviteRequest.DeletedByID = userID;
                    inviteRequest.DeletedDate = DateTime.UtcNow;
                    this.inviteRequestRepository.Update(inviteRequest);
                    this.inviteRequestRepository.SaveChanges();

                    operationStatus.Succeeded = true;
                }
                else
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.NoPermissionInviteRequestMessage);
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.UnknownErrorMessage;
            }

            return operationStatus;
        }

        /// <summary>
        /// Adds the Join request of the user to a community for the given role.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <returns>True if the request is added, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus JoinCommunity(PermissionItem permissionItem)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            OperationStatus operationStatus = new OperationStatus();

            try
            {
                PermissionRequest permissionRequest = new PermissionRequest();
                Mapper.Map(permissionItem, permissionRequest);

                permissionRequest.RequestedDate = DateTime.UtcNow;

                this.permissionRequestRepository.Add(permissionRequest);
                this.permissionRequestRepository.SaveChanges();
                operationStatus.Succeeded = true;
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.UnknownErrorMessage;
            }

            return operationStatus;
        }

        /// <summary>
        /// Joins the current user to community for which the invite request token was generated.
        /// </summary>
        /// <param name="userID">User who is making the join request</param>
        /// <param name="inviteRequestToken">Token to be used for joining the community</param>
        /// <returns>Status of the operation. Success, if succeeded, failure message and exception details in case of exception.</returns>
        public OperationStatus JoinCommunity(long userID, Guid inviteRequestToken)
        {
            OperationStatus operationStatus = new OperationStatus();

            try
            {
                // Find the invite request entity in database.
                InviteRequest inviteRequest = this.inviteRequestRepository.GetItem(invite => invite.InviteRequestToken == inviteRequestToken, "InviteRequestContent");

                if (inviteRequest == null || inviteRequest.IsDeleted == true)
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.InviteDeletedErrorMessage);
                }
                else if (inviteRequest.Used == true)
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.InviteUsedErrorMessage);
                }
                else
                {
                    PermissionItem permissionItem = new PermissionItem();
                    permissionItem.UserID = userID;
                    permissionItem.CommunityID = inviteRequest.InviteRequestContent.CommunityID;
                    permissionItem.Role = (UserRole)inviteRequest.InviteRequestContent.RoleID;

                    // Check if at all the user is already member of the same community.
                    var existingRole = this.userCommunitiesRepository.GetItem(
                                                    userCommunity => userCommunity.UserID == userID && userCommunity.CommunityId == inviteRequest.InviteRequestContent.CommunityID);

                    if (existingRole == null || inviteRequest.InviteRequestContent.RoleID > existingRole.RoleID)
                    {
                        operationStatus = this.userCommunitiesRepository.UpdateUserRoles(permissionItem);
                    }
                    else
                    {
                        // Just mark OperationStatus as succeeded so that, the token will be marked as used.
                        operationStatus.Succeeded = true;
                    }

                    if (operationStatus.Succeeded)
                    {
                        inviteRequest.Used = true;
                        inviteRequest.UsedByID = userID;
                        inviteRequest.UsedDate = DateTime.UtcNow;
                        this.inviteRequestRepository.Update(inviteRequest);
                        this.inviteRequestRepository.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.UnknownErrorMessage;
            }

            return operationStatus;
        }

        /// <summary>
        /// Approves or declines a permission request of a user for a community.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedByID">User who is updating the permission request</param>
        /// <returns>True if the request is updated, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus UpdateUserPermissionRequest(PermissionItem permissionItem, long updatedByID)
        {
            OperationStatus operationStatus = new OperationStatus();

            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            try
            {
                // Need to check the current user role before updating the request.
                UserRole currentUserRole = this.userRepository.GetUserRole(updatedByID, permissionItem.CommunityID);

                // 1. User should be having moderator role or higher.
                // 2. If the permission being assigned is Owner, then only owners or site administrators can update the permission.
                if (currentUserRole < UserRole.Moderator ||
                        (permissionItem.Role == UserRole.Owner && currentUserRole != UserRole.Owner && currentUserRole != UserRole.SiteAdmin))
                {
                    operationStatus.Succeeded = false;
                    operationStatus.CustomErrorMessage = true;
                    operationStatus.ErrorMessage = Resources.NoPermissionsErrorMessage;
                }
                else
                {
                    operationStatus = this.userCommunitiesRepository.UpdateUserPermissionRequest(permissionItem, updatedByID);
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.UnknownErrorMessage;
            }

            return operationStatus;
        }

        /// <summary>
        /// Updates (changing the role or deleting the role) the permission request for a user for a community.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedByID">User who is updating the permission request</param>
        /// <returns>True if the permission is updated, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus UpdateUserRoles(PermissionItem permissionItem, long updatedByID)
        {
            OperationStatus operationStatus = new OperationStatus();

            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            try
            {
                // Need to check the current user role before updating the request.
                UserRole currentUserRole = this.userRepository.GetUserRole(updatedByID, permissionItem.CommunityID);

                // 1. Leave community should check for user role.
                // 2. User should be having moderator role or higher.
                // 3. If the permission being assigned in Owner, then only owners or site administrators can update the permission.
                if (permissionItem.Role != UserRole.None && (
                        currentUserRole < UserRole.Moderator ||
                                (permissionItem.Role == UserRole.Owner && currentUserRole != UserRole.Owner && currentUserRole != UserRole.SiteAdmin)))
                {
                    operationStatus.Succeeded = false;
                    operationStatus.CustomErrorMessage = true;
                    operationStatus.ErrorMessage = Resources.NoPermissionsErrorMessage;
                }
                else
                {
                    operationStatus = this.userCommunitiesRepository.UpdateUserRoles(permissionItem);
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.UnknownErrorMessage;
            }

            return operationStatus;
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
            return this.userRepository.GetLatestProfileIDs(count);
        }

        /// <summary>
        /// Checks if the user is Site Admin
        /// </summary>
        /// <param name="userID">ID of the user.</param>
        /// <returns>True if user is site admin;Otherwise false.</returns>
        public OperationStatus IsSiteAdmin(long userID)
        {
            OperationStatus operationStatus = null;
            try
            {
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    operationStatus = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                operationStatus = OperationStatus.CreateFailureStatus(exception);
            }

            return operationStatus;
        }

        /// <summary>
        /// This function is used to get all profiles in the database excluding the current user.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="userID">ID the of the current user.</param>
        /// <returns>List of all profile in database.</returns>
        public IEnumerable<ProfileDetails> GetAllProfiles(long userID)
        {
            List<ProfileDetails> profiles = new List<ProfileDetails>();
            if (this.userRepository.IsSiteAdmin(userID))
            {
                var users = this.userRepository.GetItems(user => user.UserID != userID, user => user.LastName, false);

                foreach (var item in users)
                {
                    var profileDetails = new ProfileDetails();
                    Mapper.Map(item, profileDetails);
                    profiles.Add(profileDetails);
                }
            }

            return profiles;
        }

        /// <summary>
        /// This function is used to get all profiles in the database including the current user.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="userID">ID the of the current user.</param>
        /// <returns>List of all profile in database.</returns>
        public IEnumerable<AdminReportProfileDetails> GetAllProfilesForReport(long userID)
        {
            List<AdminReportProfileDetails> profiles = new List<AdminReportProfileDetails>();
            if (this.userRepository.IsSiteAdmin(userID))
            {
                var users = this.userRepository.GetItems(user => user.IsDeleted != true, user => user.LastName, false);

                var communities = this.communitiesViewRepository.GetItems(c => c.CommunityTypeID != (int)CommunityTypes.User, null, false).Select(c => new { c.CommunityID, c.CreatedByID, c.CreatedDatetime });
                var contents = this.contentsViewRepository.GetItems(null, null, false).Select(c => new { c.ContentID, c.CreatedByID, c.CreatedDatetime });

                foreach (var item in users)
                {
                    var profileDetails = new AdminReportProfileDetails();
                    Mapper.Map(item, profileDetails);

                    // Update Total contents and communities.
                    var userContents = contents.Where(c => c.CreatedByID == item.UserID).OrderByDescending(c => c.CreatedDatetime);
                    if (userContents != null && userContents.LongCount() > 0)
                    {
                        profileDetails.TotalContents = userContents.LongCount();
                        profileDetails.LastUploaded = userContents.FirstOrDefault().CreatedDatetime.Value;
                        profileDetails.TotalUsedSize = this.contentsViewRepository.GetConsumedSize(item.UserID);
                    }

                    profileDetails.TotalCommunities = communities.Where(c => c.CreatedByID == item.UserID).LongCount();

                    profiles.Add(profileDetails);
                }
            }

            return profiles;
        }

        /// <summary>
        /// This function is used to promote the users with the ID specified as Site administrators.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="adminUsers">Admin user list who has to be promoted to site administrators.</param>
        /// <param name="updatedByID">ID the of the current user.</param>
        /// <returns>True if the Users has been promoted.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus PromoteAsSiteAdmin(IEnumerable<long> adminUsers, long updatedByID)
        {
            OperationStatus operationStatus = null;

            try
            {
                if (this.userRepository.IsSiteAdmin(updatedByID))
                {
                    var userTypes = this.userTypeRepository.GetAll(null);

                    // Mark the user who have been demoted from the site administrators as Regular user's.
                    Expression<Func<User, bool>> removedUserCondition = (user) => (user.UserID != updatedByID && user.UserTypeID == (int)UserTypes.SiteAdmin && !adminUsers.Contains(user.UserID));
                    var removedAdmins = this.userRepository.GetItems(removedUserCondition, null, false);
                    foreach (var removedUser in removedAdmins)
                    {
                        removedUser.UserType = userTypes.Where(type => type.UserTypeID == (int)UserTypes.Regular).FirstOrDefault();
                        this.userRepository.Update(removedUser);
                    }

                    // Mark the user who have been promoted as site administrators.
                    Expression<Func<User, bool>> promotedUserCondition = (user) => (user.UserID != updatedByID && adminUsers.Contains(user.UserID) && user.UserTypeID != (int)UserTypes.SiteAdmin);
                    var promotedAdmins = this.userRepository.GetItems(promotedUserCondition, null, false);
                    foreach (var promotedUser in promotedAdmins)
                    {
                        promotedUser.UserType = userTypes.Where(type => type.UserTypeID == (int)UserTypes.SiteAdmin).FirstOrDefault();
                        this.userRepository.Update(promotedUser);
                    }

                    this.userRepository.SaveChanges();
                }
                else
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                operationStatus = OperationStatus.CreateFailureStatus(exception);
            }

            operationStatus = operationStatus ?? OperationStatus.CreateSuccessStatus();

            return operationStatus;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Moves thumbnail from temporary storage to thumbnail storage in azure.
        /// </summary>
        /// <param name="profileImageId">Guid of the temporary profile image.</param>
        private Guid? MoveThumbnail(Guid? profileImageId)
        {
            Guid? thumbnailId = null;
            if (profileImageId.HasValue && !profileImageId.Equals(Guid.Empty))
            {
                BlobDetails thumbnailBlob = new BlobDetails()
                {
                    BlobID = profileImageId.ToString()
                };

                thumbnailId = this.blobDataRepository.MoveThumbnail(thumbnailBlob) ? profileImageId.Value : Guid.Empty;
            }

            return thumbnailId;
        }

        /// <summary>
        /// Deletes Thumbnail from azure.
        /// </summary>
        /// <param name="azureID">Id of the thumbnail to be deleted.</param>
        private void DeleteThumbnail(Guid? azureID)
        {
            if (azureID.HasValue && !azureID.Equals(Guid.Empty))
            {
                BlobDetails fileBlob = new BlobDetails()
                {
                    BlobID = azureID.ToString(),
                };

                // Delete file from azure.
                this.blobDataRepository.DeleteThumbnail(fileBlob);
            }
        }

        #endregion
    }
}

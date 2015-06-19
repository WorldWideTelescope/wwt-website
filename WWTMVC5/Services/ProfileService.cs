//-----------------------------------------------------------------------
// <copyright file="ProfileService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        private IUserRepository _userRepository;

        /// <summary>
        /// Instance of Contents repository
        /// </summary>
        private IContentsViewRepository _contentsViewRepository;

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunitiesViewRepository _communitiesViewRepository;

        /// <summary>
        /// Instance of UserCommunities repository
        /// </summary>
        private IUserCommunitiesRepository _userCommunitiesRepository;

        /// <summary>
        /// Instance of Permission repository
        /// </summary>
        private IRepositoryBase<PermissionRequest> _permissionRequestRepository;

        /// <summary>
        /// Instance of Blob data repository
        /// </summary>
        private IBlobDataRepository _blobDataRepository;

        /// <summary>
        /// Instance of InviteRequestsView repository
        /// </summary>
        private IRepositoryBase<InviteRequestsView> _inviteRequestsViewRepository;

        /// <summary>
        /// Instance of InviteRequest repository
        /// </summary>
        private IRepositoryBase<InviteRequest> _inviteRequestRepository;

        /// <summary>
        /// Instance of UserType repository
        /// </summary>
        private IRepositoryBase<UserType> _userTypeRepository;

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
            _userRepository = userRepository;
            _contentsViewRepository = contentsViewRepository;
            _communitiesViewRepository = communitiesViewRepository;
            _userCommunitiesRepository = userCommunitiesRepository;
            _permissionRequestRepository = permissionRequestRepository;
            _blobDataRepository = blobDataRepository;
            _inviteRequestsViewRepository = inviteRequestsViewRepository;
            _inviteRequestRepository = inviteRequestRepository;
            _userTypeRepository = userTypeRepository;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Gets the user profile from the puid
        /// </summary>
        /// <param name="puid">Live user PUID</param>
        /// <returns>ProfileDetails object</returns>
        public async Task<ProfileDetails> GetProfileAsync(string puid)
        {
            ProfileDetails profileDetails = null;
            var userDetails = await _userRepository.GetItemAsync(user => user.LiveID == puid);

            // Check if the user details is present.
            // DO NOT use CheckNotNull since GetProfile should not throw exception.
            if (userDetails != null)
            {
                profileDetails = new ProfileDetails();
                Mapper.Map(userDetails, profileDetails);

                // Set the consumed size
                profileDetails.ConsumedSize = _contentsViewRepository.GetConsumedSize(userDetails.UserID);
            }

            return profileDetails;
        }
        public ProfileDetails GetProfile(string puid)
        {
            ProfileDetails profileDetails = null;
            var userDetails = _userRepository.GetItem(user => user.LiveID == puid);

            // Check if the user details is present.
            // DO NOT use CheckNotNull since GetProfile should not throw exception.
            if (userDetails != null)
            {
                profileDetails = new ProfileDetails();
                Mapper.Map(userDetails, profileDetails);

                // Set the consumed size
                profileDetails.ConsumedSize = _contentsViewRepository.GetConsumedSize(userDetails.UserID);
            }

            return profileDetails;
        }

        /// <summary>
        /// Gets the user profile from USER ID
        /// </summary>
        /// <param name="userId">User profile ID</param>
        /// <returns>ProfileDetails object</returns>
        public ProfileDetails GetProfile(long userId)
        {
            var profileDetails = new ProfileDetails();
            var userDetails = _userRepository.GetItem(user => user.UserID == userId, "UserType");

            // Check if the user details is present.
            this.CheckNotNull(() => new { userDetails });

            Mapper.Map(userDetails, profileDetails);

            // Set the consumed size
            profileDetails.ConsumedSize = _contentsViewRepository.GetConsumedSize(userId);

            return profileDetails;
        }

        /// <summary>
        /// Gets the user profile details for the list of user ID
        /// </summary>
        /// <param name="users">User profile ID list</param>
        /// <returns>ProfileDetails objects list</returns>
        public async Task<IEnumerable<ProfileDetails>> GetProfilesAsync(IEnumerable<long> users)
        {
            var profileDetails = new List<ProfileDetails>();
            Expression<Func<User, bool>> condition = (user) => (users.Contains(user.UserID));
            var userDetails = _userRepository.GetItems(condition, null, false);

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
        public IEnumerable<ProfileDetails> GetProfiles(IEnumerable<long> users)
        {
            var profileDetails = new List<ProfileDetails>();
            Expression<Func<User, bool>> condition = (user) => (users.Contains(user.UserID));
            var userDetails = _userRepository.GetItems(condition, null, false);

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
        public async Task<bool> UpdateProfileAsync(ProfileDetails profile)
        {
            var status = false;
            var userDetails = _userRepository.GetItem(user => user.UserID == profile.ID);
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
                    _userRepository.Update(userDetails);

                    // Save all the changes made.
                    _userRepository.SaveChanges();

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
        public bool UpdateProfile(ProfileDetails profile)
        {
            var status = false;
            var userDetails = _userRepository.GetItem(user => user.UserID == profile.ID);
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
                    _userRepository.Update(userDetails);

                    // Save all the changes made.
                    _userRepository.SaveChanges();

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
        /// <param name="userId">Communities Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all communities</returns>
        public async Task<IEnumerable<CommunityDetails>> GetCommunitiesAsync(long userId, PageDetails pageDetails, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId, pageDetails });

            IList<CommunityDetails> userCommunities = new List<CommunityDetails>();

            Func<CommunitiesView, object> orderBy = c => c.LastUpdatedDatetime;

            // Get all the community ids to which user is given role of contributor or higher.
            var userCommunityIds = _userRepository.GetUserCommunitiesForRole(userId, UserRole.Contributor, onlyPublic);

            // Get only the communities for the current page.
            userCommunityIds = userCommunityIds.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);

            Expression<Func<CommunitiesView, bool>> condition = c => userCommunityIds.Contains(c.CommunityID);

            foreach (var community in _communitiesViewRepository.GetItems(condition, orderBy, true))
            {
                CommunityDetails communityDetails;
                if (onlyPublic)
                {
                    // In case of only public, user is looking at somebody else profile and so just send the user role as Visitor.
                    communityDetails = new CommunityDetails(Permission.Visitor);
                }
                else
                {
                    var userRole = _userRepository.GetUserRole(userId, community.CommunityID);
                    communityDetails = new CommunityDetails(userRole.GetPermission());
                }

                Mapper.Map(community, communityDetails);
                userCommunities.Add(communityDetails);
            }

            return userCommunities;
        }
        public IEnumerable<CommunityDetails> GetCommunities(long userId, PageDetails pageDetails, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId, pageDetails });

            IList<CommunityDetails> userCommunities = new List<CommunityDetails>();

            Func<CommunitiesView, object> orderBy = c => c.LastUpdatedDatetime;

            // Get all the community ids to which user is given role of contributor or higher.
            var userCommunityIds = _userRepository.GetUserCommunitiesForRole(userId, UserRole.Contributor, onlyPublic);

            // Get only the communities for the current page.
            userCommunityIds = userCommunityIds.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);

            Expression<Func<CommunitiesView, bool>> condition = c => userCommunityIds.Contains(c.CommunityID);

            foreach (var community in _communitiesViewRepository.GetItems(condition, orderBy, true))
            {
                CommunityDetails communityDetails;
                if (onlyPublic)
                {
                    // In case of only public, user is looking at somebody else profile and so just send the user role as Visitor.
                    communityDetails = new CommunityDetails(Permission.Visitor);
                }
                else
                {
                    var userRole = _userRepository.GetUserRole(userId, community.CommunityID);
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
        /// <param name="userId">Contents Owner ID.</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>List of all contents</returns>
        public async Task<IEnumerable<ContentDetails>> GetContentsAsync(long userId, PageDetails pageDetails, bool onlyPublic)
        {
            IList<ContentDetails> contents = new List<ContentDetails>();
            Expression<Func<ContentsView, bool>> condition;

            this.CheckNotNull(() => new { userID = userId, pageDetails });

            Func<ContentsView, object> orderBy = c => c.LastUpdatedDatetime;

            if (onlyPublic)
            {
                var accessType = AccessType.Public.ToString();
                condition = c => c.CreatedByID == userId && c.AccessType == accessType;
            }
            else
            {
                condition = c => c.CreatedByID == userId;
            }

            foreach (var content in _contentsViewRepository.GetItems(
                                                condition, orderBy, true, ((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage), pageDetails.ItemsPerPage))
            {
                var contentDetails = onlyPublic ?
                    new ContentDetails(Permission.Visitor) :
                    new ContentDetails(Permission.Contributor);

                Mapper.Map(content, contentDetails);
                contents.Add(contentDetails);
            }

            return contents;
        }
        public IEnumerable<ContentDetails> GetContents(long userId, PageDetails pageDetails, bool onlyPublic)
        {
            IList<ContentDetails> contents = new List<ContentDetails>();
            Expression<Func<ContentsView, bool>> condition;

            this.CheckNotNull(() => new { userID = userId, pageDetails });

            Func<ContentsView, object> orderBy = c => c.LastUpdatedDatetime;

            if (onlyPublic)
            {
                var accessType = AccessType.Public.ToString();
                condition = c => c.CreatedByID == userId && c.AccessType == accessType;
            }
            else
            {
                condition = c => c.CreatedByID == userId;
            }

            foreach (var content in _contentsViewRepository.GetItems(
                                                condition, orderBy, true, ((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage), pageDetails.ItemsPerPage))
            {
                var contentDetails = onlyPublic ?
                    new ContentDetails(Permission.Visitor) :
                    new ContentDetails(Permission.Contributor);

                Mapper.Map(content, contentDetails);
                contents.Add(contentDetails);
            }

            return contents;
        }

        /// <summary>
        /// Gets the total number of communities for the given user.
        /// </summary>
        /// <param name="userId">Communities owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of communities.</returns>
        public async Task<int> GetCommunitiesCountAsync(long userId, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId });

            return _userRepository.GetUserCommunitiesForRole(userId, UserRole.Contributor, onlyPublic).Count();
        }
        public int GetCommunitiesCount(long userId, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId });

            return _userRepository.GetUserCommunitiesForRole(userId, UserRole.Contributor, onlyPublic).Count();
        }

        /// <summary>
        /// Gets the total number of contents for the given user.
        /// </summary>
        /// <param name="userId">Contents Owner ID.</param>
        /// <param name="onlyPublic">Whether to only retrieve public data</param>
        /// <returns>total number of contents.</returns>
        public async Task<int> GetContentsCountAsync(long userId, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId });

            Expression<Func<ContentsView, bool>> condition = null;
            if (onlyPublic)
            {
                var accessType = AccessType.Public.ToString();
                condition = c => c.CreatedByID == userId && c.AccessType == accessType;
            }
            else
            {
                condition = c => c.CreatedByID == userId;
            }

            return _contentsViewRepository.GetItemsCount(condition);
        }
        public int GetContentsCount(long userId, bool onlyPublic)
        {
            this.CheckNotNull(() => new { userID = userId });

            Expression<Func<ContentsView, bool>> condition = null;
            if (onlyPublic)
            {
                var accessType = AccessType.Public.ToString();
                condition = c => c.CreatedByID == userId && c.AccessType == accessType;
            }
            else
            {
                condition = c => c.CreatedByID == userId;
            }

            return _contentsViewRepository.GetItemsCount(condition);
        }

        /// <summary>
        /// Created the user profile
        /// </summary>
        /// <param name="profileDetails">ProfileDetails object</param>
        /// <returns>Profile ID</returns>
        public async Task<long> CreateProfileAsync(ProfileDetails profileDetails)
        {
            // Make sure communityDetails is not null
            this.CheckNotNull(() => new { profileDetails });

            // Check if the user already exists in the Layerscape database.
            User existingUser = _userRepository.GetItem(u => u.LiveID == profileDetails.PUID);

            if (existingUser != null)
            {
                return existingUser.UserID;
            }
            else
            {
                // 1. Add Community details to the community object.
                var user = new User();
                Mapper.Map(profileDetails, user);

                user.JoinedDateTime = DateTime.UtcNow;
                user.LastLoginDatetime = DateTime.UtcNow;

                // While creating the user, IsDeleted to be false always.
                user.IsDeleted = false;

                // Add the user to the repository
                _userRepository.Add(user);

                // Save all the changes made.
                _userRepository.SaveChanges();

                return user.UserID;
            }
        }
        public long CreateProfile(ProfileDetails profileDetails)
        {
            // Make sure communityDetails is not null
            this.CheckNotNull(() => new { profileDetails });

            // Check if the user already exists in the Layerscape database.
            User existingUser = _userRepository.GetItem(u => u.LiveID == profileDetails.PUID);

            if (existingUser != null)
            {
                return existingUser.UserID;
            }
            else
            {
                // 1. Add Community details to the community object.
                var user = new User();
                Mapper.Map(profileDetails, user);

                user.JoinedDateTime = DateTime.UtcNow;
                user.LastLoginDatetime = DateTime.UtcNow;

                // While creating the user, IsDeleted to be false always.
                user.IsDeleted = false;

                // Add the user to the repository
                _userRepository.Add(user);

                // Save all the changes made.
                _userRepository.SaveChanges();

                return user.UserID;
            }
        }

        /// <summary>
        /// Gets the user permissions for the given community and for the given page. User should have at least
        /// contributor permission on the community to get user permissions.
        /// </summary>
        /// <param name="userId">User who is reading the permissions</param>
        /// <param name="communityId">Community for which permissions are fetched</param>
        /// <param name="pageDetails">Page for which permissions are fetched</param>
        /// <returns>List of permissions/user roles</returns>
        
        public async Task<PermissionDetails> GetUserPemissions(long userId, long communityId, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            Expression<Func<UserCommunities, bool>> condition = c => c.CommunityId == communityId;
            Func<UserCommunities, object> orderBy = c => c.RoleID;

            // Gets the total items satisfying the condition
            pageDetails.TotalCount =  _userCommunitiesRepository.GetItemsCount(condition);
            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // TODO: Passing the condition in a variable doesn't add the WHERE clause in SQL server. Need to work on this later.
            
            var items =  _userCommunitiesRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            var permissionDetails = new PermissionDetails();

            if (items != null && items.Any())
            {
                var userRole = _userRepository.GetUserRole(userId, communityId);

                // User has to be at least contributor to know the permission details of the community.
                if (userRole >= UserRole.Contributor)
                {
                    permissionDetails.CurrentUserPermission = userRole.GetPermission();

                    foreach (var item in items)
                    {
                        var permissionItem = new PermissionItem();
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
        /// <param name="userId">User who is reading the requests</param>
        /// <param name="communityId">Community for which requests are fetched</param>
        /// <param name="pageDetails">Page for which requests are fetched</param>
        /// <returns>List of user role requests</returns>
        public async Task<PermissionDetails> GetUserPemissionRequests(long userId, long? communityId, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            // Condition to get all the pending requests irrespective of community.
            Expression<Func<PermissionRequest, bool>> condition = (PermissionRequest pr) => pr.Approved == null;
            Func<PermissionRequest, object> orderBy = (PermissionRequest c) => c.RoleID;

            if (communityId.HasValue)
            {
                // If community is specified, get all the pending requests of the specified community.
                condition = (PermissionRequest pr) => pr.Approved == null && pr.CommunityID == communityId.Value;
            }
            else
            {
                // If no community id is specified, get all the community ids to which user is given role of moderator or 
                // higher and get their pending requests.
                var userCommunityIds = _userRepository.GetUserCommunitiesForRole(userId, UserRole.Moderator, false);

                condition = (PermissionRequest pr) => pr.Approved == null && userCommunityIds.Contains(pr.CommunityID);
            }

            // Gets the total items satisfying the condition
            pageDetails.TotalCount =  _permissionRequestRepository.GetItemsCount(condition);
            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            var permissionDetails = new PermissionDetails();

            foreach (var item in  _permissionRequestRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage))
            {
                var userRole = _userRepository.GetUserRole(userId, item.CommunityID);

                // 1. User has to be at least Moderator to know the permission request details of the community.
                // 2. In case of profile page, user might be moderator for few communities and not for others. So, need to send only the requests
                //    of community to which user is moderator or higher.
                if (userRole >= UserRole.Moderator)
                {
                    var permissionItem = new PermissionItem();
                    Mapper.Map(item, permissionItem);
                    permissionItem.CurrentUserRole = userRole;
                    permissionDetails.PermissionItemList.Add(permissionItem);
                    permissionDetails.CurrentUserPermission = userRole.GetPermission();
                }
                else if (communityId.HasValue)
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
        /// <param name="userId">User who is reading the invite requests</param>
        /// <param name="communityId">Community for which invite requests are fetched</param>
        /// <param name="pageDetails">Page for which invite requests are fetched</param>
        /// <returns>List of open invite requests for the community</returns>
        public async Task<IEnumerable<InviteRequestItem>> GetInviteRequests(long userId, long communityId, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            IList<InviteRequestItem> inviteRequestItemList = new List<InviteRequestItem>();

            var userRole = _userRepository.GetUserRole(userId, communityId);
            if (userRole >= UserRole.Moderator)
            {
                // Condition to get all the pending requests irrespective of community.
                Expression<Func<InviteRequestsView, bool>> condition = (InviteRequestsView invite) => invite.Used == false && invite.CommunityID == communityId;
                Func<InviteRequestsView, object> orderBy = (InviteRequestsView invite) => invite.InvitedDate;

                // Gets the total items satisfying the condition
                pageDetails.TotalCount =   _inviteRequestsViewRepository.GetItemsCount(condition);
                pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

                foreach (var item in  _inviteRequestsViewRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage))
                {
                    var inviteRequestItem = new InviteRequestItem();
                    Mapper.Map(item, inviteRequestItem);

                    inviteRequestItemList.Add(inviteRequestItem);
                }
            }

            return inviteRequestItemList;
        }

        /// <summary>
        /// Removes the specified invite request.
        /// </summary>
        /// <param name="userId">User who is removing the invite request</param>
        /// <param name="inviteRequestId">Invite request to be removed</param>
        /// <returns>True if the invite request is removed, false otherwise</returns>
        public async Task<OperationStatus> RemoveInviteRequest(long userId, int inviteRequestId)
        {
            var operationStatus = new OperationStatus();

            try
            {
                // Find the invite request entity in database.
                InviteRequest inviteRequest =  _inviteRequestRepository.GetItem(ir => ir.InviteRequestID == inviteRequestId, "InviteRequestContent");

                // Check invite request is not null
                this.CheckNotNull(() => new { inviteRequest });

                var userRole = _userRepository.GetUserRole(userId, inviteRequest.InviteRequestContent.CommunityID);
                if (userRole >= UserRole.Moderator)
                {
                    inviteRequest.IsDeleted = true;
                    inviteRequest.DeletedByID = userId;
                    inviteRequest.DeletedDate = DateTime.UtcNow;
                    _inviteRequestRepository.Update(inviteRequest);
                    _inviteRequestRepository.SaveChanges();

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
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus JoinCommunity(PermissionItem permissionItem)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            var operationStatus = new OperationStatus();

            try
            {
                var permissionRequest = new PermissionRequest();
                Mapper.Map(permissionItem, permissionRequest);

                permissionRequest.RequestedDate = DateTime.UtcNow;

                _permissionRequestRepository.Add(permissionRequest);
                _permissionRequestRepository.SaveChanges();
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
        /// <param name="userId">User who is making the join request</param>
        /// <param name="inviteRequestToken">Token to be used for joining the community</param>
        /// <returns>Status of the operation. Success, if succeeded, failure message and exception details in case of exception.</returns>
        public async Task<OperationStatus> JoinCommunity(long userId, Guid inviteRequestToken)
        {
            var operationStatus = new OperationStatus();

            try
            {
                // Find the invite request entity in database.
                var inviteRequest =  _inviteRequestRepository.GetItem(invite => invite.InviteRequestToken == inviteRequestToken, "InviteRequestContent");

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
                    var permissionItem = new PermissionItem
                    {
                        UserID = userId,
                        CommunityID = inviteRequest.InviteRequestContent.CommunityID,
                        Role = (UserRole) inviteRequest.InviteRequestContent.RoleID
                    };

                    // Check if at all the user is already member of the same community.
                    var existingRole =  _userCommunitiesRepository.GetItem(
                                                    userCommunity => userCommunity.UserID == userId && userCommunity.CommunityId == inviteRequest.InviteRequestContent.CommunityID);

                    if (existingRole == null || inviteRequest.InviteRequestContent.RoleID > existingRole.RoleID)
                    {
                        operationStatus = _userCommunitiesRepository.UpdateUserRoles(permissionItem);
                    }
                    else
                    {
                        // Just mark OperationStatus as succeeded so that, the token will be marked as used.
                        operationStatus.Succeeded = true;
                    }

                    if (operationStatus.Succeeded)
                    {
                        inviteRequest.Used = true;
                        inviteRequest.UsedByID = userId;
                        inviteRequest.UsedDate = DateTime.UtcNow;
                        _inviteRequestRepository.Update(inviteRequest);
                        _inviteRequestRepository.SaveChanges();
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
        /// <param name="updatedById">User who is updating the permission request</param>
        /// <returns>True if the request is updated, false otherwise</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus UpdateUserPermissionRequest(PermissionItem permissionItem, long updatedById)
        {
            var operationStatus = new OperationStatus();

            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            try
            {
                // Need to check the current user role before updating the request.
                var currentUserRole = _userRepository.GetUserRole(updatedById, permissionItem.CommunityID);

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
                    operationStatus = _userCommunitiesRepository.UpdateUserPermissionRequest(permissionItem, updatedById);
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
        /// <param name="updatedById">User who is updating the permission request</param>
        /// <returns>True if the permission is updated, false otherwise</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus UpdateUserRoles(PermissionItem permissionItem, long updatedById)
        {
            var operationStatus = new OperationStatus();

            // Make sure input is not null
            this.CheckNotNull(() => new { permissionItem });

            try
            {
                // Need to check the current user role before updating the request.
                var currentUserRole = _userRepository.GetUserRole(updatedById, permissionItem.CommunityID);

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
                    operationStatus = _userCommunitiesRepository.UpdateUserRoles(permissionItem);
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
            return _userRepository.GetLatestProfileIDs(count);
        }

        /// <summary>
        /// Checks if the user is Site Admin
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>True if user is site admin;Otherwise false.</returns>
        public OperationStatus IsSiteAdmin(long userId)
        {
            OperationStatus operationStatus = null;
            try
            {
                if (_userRepository.IsSiteAdmin(userId))
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
        /// <param name="userId">ID the of the current user.</param>
        /// <returns>List of all profile in database.</returns>
        public async Task<IEnumerable<ProfileDetails>> GetAllProfiles(long userId)
        {
            var profiles = new List<ProfileDetails>();
            if (_userRepository.IsSiteAdmin(userId))
            {
                var users = _userRepository.GetItems(user => user.UserID != userId, user => user.LastName, false);

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
        /// This function is used to promote the users with the ID specified as Site administrators.
        ///     This operation can be only performed by a site admin.
        /// </summary>
        /// <param name="adminUsers">Admin user list who has to be promoted to site administrators.</param>
        /// <param name="updatedById">ID the of the current user.</param>
        /// <returns>True if the Users has been promoted.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public async Task<OperationStatus> PromoteAsSiteAdmin(IEnumerable<long> adminUsers, long updatedById)
        {
            OperationStatus operationStatus = null;

            try
            {
                if (_userRepository.IsSiteAdmin(updatedById))
                {
                    var userTypes = _userTypeRepository.GetAll(null);
                    
                    // Mark the user who have been demoted from the site administrators as Regular user's.
                    Expression<Func<User, bool>> removedUserCondition = (user) => (user.UserID != updatedById && user.UserTypeID == (int)UserTypes.SiteAdmin && !adminUsers.Contains(user.UserID));
                    var removedAdmins = _userRepository.GetItems(removedUserCondition, null, false);
                    IEnumerable<UserType> userTypesEnumerable = userTypes as UserType[] ?? userTypes.ToArray();
                    foreach (var removedUser in removedAdmins)
                    {
                        removedUser.UserType = userTypesEnumerable.FirstOrDefault(type => type.UserTypeID == (int)UserTypes.Regular);
                        _userRepository.Update(removedUser);
                    }

                    // Mark the user who have been promoted as site administrators.
                    Expression<Func<User, bool>> promotedUserCondition = (user) => (user.UserID != updatedById && adminUsers.Contains(user.UserID) && user.UserTypeID != (int)UserTypes.SiteAdmin);
                    var promotedAdmins = _userRepository.GetItems(promotedUserCondition, null, false);
                    foreach (var promotedUser in promotedAdmins)
                    {
                        promotedUser.UserType = userTypesEnumerable.FirstOrDefault(type => type.UserTypeID == (int)UserTypes.SiteAdmin);
                        _userRepository.Update(promotedUser);
                    }

                    _userRepository.SaveChanges();
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
                var thumbnailBlob = new BlobDetails()
                {
                    BlobID = profileImageId.ToString()
                };

                thumbnailId = _blobDataRepository.MoveThumbnail(thumbnailBlob) ? profileImageId.Value : Guid.Empty;
            }

            return thumbnailId;
        }

        /// <summary>
        /// Deletes Thumbnail from azure.
        /// </summary>
        /// <param name="azureId">Id of the thumbnail to be deleted.</param>
        private void DeleteThumbnail(Guid? azureId)
        {
            if (azureId.HasValue && !azureId.Equals(Guid.Empty))
            {
                var fileBlob = new BlobDetails()
                {
                    BlobID = azureId.ToString(),
                };

                // Delete file from azure.
                _blobDataRepository.DeleteThumbnail(fileBlob);
            }
        }

        #endregion
    }
}

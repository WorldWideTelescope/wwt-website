//-----------------------------------------------------------------------
// <copyright file="CommunityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Community service having methods for retrieving community
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class CommunityService : PermissionService, ICommunityService
    {
        #region Private Variables

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunityRepository communityRepository;

        /// <summary>
        /// Instance of Tag repository
        /// </summary>
        private IRepositoryBase<Tag> tagRepository;

        /// <summary>
        /// Instance of Blob data repository
        /// </summary>
        private IBlobDataRepository blobDataRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository userRepository;

        /// <summary>
        /// Instance of Content Service
        /// </summary>
        private IContentService contentService;

        /// <summary>
        /// Instance of UserCommunities repository
        /// </summary>
        private IUserCommunitiesRepository userCommunitiesRepository;

        /// <summary>
        /// Instance of OffensiveCommunities repository
        /// </summary>
        private IRepositoryBase<OffensiveCommunities> offensiveCommunitiesRepository;

        /// <summary>
        /// Instance of OffensiveContent repository
        /// </summary>
        private IRepositoryBase<OffensiveContent> offensiveContentRepository;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommunityService class.
        /// </summary>
        /// <param name="communityRepository">Instance of Community repository</param>
        /// <param name="tagRepository">Instance of Tag repository</param>
        /// <param name="blobDataRepository">Instance of Blob data repository</param>
        /// /// <param name="userRepository">Instance of User repository</param>
        public CommunityService(
            ICommunityRepository communityRepository,
            IRepositoryBase<Tag> tagRepository,
            IBlobDataRepository blobDataRepository,
            IUserRepository userRepository,
            IUserCommunitiesRepository userCommunitiesRepository,
            IRepositoryBase<OffensiveCommunities> offensiveCommunitiesRepository,
            IRepositoryBase<OffensiveContent> offensiveContentRepository)
            : base(communityRepository, userRepository)
        {
            this.communityRepository = communityRepository;
            this.tagRepository = tagRepository;
            this.blobDataRepository = blobDataRepository;
            this.userRepository = userRepository;
            this.userCommunitiesRepository = userCommunitiesRepository;
            this.offensiveCommunitiesRepository = offensiveCommunitiesRepository;
            this.offensiveContentRepository = offensiveContentRepository;

            // TODO : Revisit this
            this.contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Gets the community identified by community ID for EDIT purpose, means user must have required role for editing the community
        /// </summary>
        /// <param name="communityID">ID of the community which has to be retrieved</param>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <returns>Instance of community details</returns>
        public CommunityDetails GetCommunityDetailsForEdit(long communityID, long userID)
        {
            CommunityDetails communityDetails = null;
            Community community = this.communityRepository.GetItem((Community c) => c.CommunityID == communityID && c.IsDeleted == false);

            // Make sure community is not null
            this.CheckNotNull(() => new { community });

            UserRole userRole = GetCommunityUserRole(community.CommunityID, userID);
            if (!CanEditDeleteCommunity(community, userID, userRole))
            {
                throw new HttpException(401, Resources.NoPermissionUpdateCommunityMessage);
            }

            communityDetails = CreateCommunityDetails(community, userID, true);
            return communityDetails;
        }

        /// <summary>
        /// Gets the community identified by community ID.
        /// </summary>
        /// <param name="communityID">
        /// ID of the community which has to be retrieved.
        /// </param>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <param name="considerPrivateCommunity">Get community details for private community also? Needed for sharing private communities</param>
        /// <param name="updateReadCount">Update the read count for the community in case of true</param>
        /// <returns>
        /// Instance of community details
        /// </returns>
        public CommunityDetails GetCommunityDetails(long communityID, long? userID, bool considerPrivateCommunity = false, bool updateReadCount = false)
        {
            CommunityDetails communityDetails = null;
            Community community = this.communityRepository.GetCommunity(communityID);

            try
            {
                communityDetails = CreateCommunityDetails(community, userID, true);

                if (updateReadCount)
                {
                    // Update the view count for the community. Note that, this line will be hit only if the user is
                    // having read access to the community.
                    IncrementViewCount(community);
                }
            }
            catch (HttpException)
            {
                if (considerPrivateCommunity)
                {
                    // In case of private community and user not having access to it, get the private community details instance
                    // which will be used only for joining the private community.
                    communityDetails = CreatePrivateCommunityDetails(community, userID);
                }
                else
                {
                    throw;
                }
            }

            return communityDetails;
        }

        /// <summary>
        /// Creates the new community in Layerscape with the given details passed in CommunitiesView instance.
        /// </summary>
        /// <param name="communityDetail">Details of the community</param>
        /// <returns>Id of the community created. Returns -1 is creation is failed.</returns>
        public long CreateCommunity(CommunityDetails communityDetail)
        {
            // Make sure communityDetails is not null
            this.CheckNotNull(() => new { communityDetails = communityDetail });

            UserRole userRole = GetCommunityUserRole(communityDetail.ParentID, communityDetail.CreatedByID);
            if (!CanCreateCommunity(communityDetail.ParentID, userRole))
            {
                throw new HttpException(401, Resources.NoPermissionCreateCommunityMessage);
            }

            // In case if the community getting created is "User" type, check that the same user already has a "User" community associated with him.
            // There should be only one "User" community to be created per user.
            if (communityDetail.CommunityType == CommunityTypes.User)
            {
                Community existingUserCommunity = this.communityRepository.GetItem(
                                                            c => c.CommunityTypeID == (int)CommunityTypes.User && c.CreatedByID == communityDetail.CreatedByID);

                if (existingUserCommunity != null)
                {
                    return existingUserCommunity.CommunityID;
                }
            }

            // 1. Add Community details to the community object.
            Community community = new Community();
            Mapper.Map(communityDetail, community);

            // While creating the community, IsDeleted to be false always.
            community.IsDeleted = false;

            community.CreatedDatetime = community.ModifiedDatetime = DateTime.UtcNow;

            // 2. Add Thumbnail to blob
            if (communityDetail.Thumbnail != null && communityDetail.Thumbnail.AzureID != Guid.Empty)
            {
                if (MoveThumbnail(communityDetail.Thumbnail))
                {
                    community.ThumbnailID = communityDetail.Thumbnail.AzureID;
                }
            }

            // 3. Add Tag details. This will also take care of creating tags if they are not there in the Layerscape database.
            this.SetCommunityTags(communityDetail.Tags, community);

            bool parentAddedToUserRole = false;
            if (communityDetail.ParentID > 0)
            {
                // 4. Add Parent Community details
                CommunityRelation communityRelation = new CommunityRelation();
                communityRelation.ParentCommunityID = communityDetail.ParentID;
                communityRelation.ChildCommunityID = communityDetail.ID;

                // TODO: Need to rename the Data Model property CommunityRelation1 with a more meaningful name.
                // Note that the relation to be added is to CommunityRelation1 since the current Community is Child.
                // When the current community is parent, it's relation to be added in CommunityRelation.
                community.CommunityRelation1.Add(communityRelation);

                var parentCommunity = this.communityRepository.GetItem(c => c.CommunityID == communityDetail.ParentID);
                if (parentCommunity != null && parentCommunity.UserCommunities.Count > 0)
                {
                    parentCommunity.ModifiedByID = communityDetail.CreatedByID;
                    parentCommunity.ModifiedDatetime = DateTime.UtcNow;

                    // 5. Inherit Parent Permission Details
                    foreach (var communityUserRole in parentCommunity.UserCommunities)
                    {
                        var userCommunityRole = new UserCommunities();

                        // Add the current community to the use role
                        userCommunityRole.CommunityId = communityDetail.ID;

                        // Add the existing users along with their roles
                        userCommunityRole.UserID = communityUserRole.UserID;
                        userCommunityRole.RoleID = communityUserRole.RoleID;
                        userCommunityRole.IsInherited = true;
                        userCommunityRole.CreatedDatetime = DateTime.UtcNow;

                        community.UserCommunities.Add(userCommunityRole);

                        if (communityUserRole.UserID == communityDetail.CreatedByID)
                        {
                            parentAddedToUserRole = true;
                        }
                    }
                }
            }

            if (!parentAddedToUserRole)
            {
                // 6. Add Owner Permission Details only if its not already inherited.
                var userCommunity = new UserCommunities();
                userCommunity.CommunityId = communityDetail.ID;
                userCommunity.UserID = communityDetail.CreatedByID;
                userCommunity.CreatedDatetime = DateTime.UtcNow;

                // User who is creating the community is the owner.
                userCommunity.RoleID = (int)UserRole.Owner;
                userCommunity.IsInherited = false;
                community.UserCommunities.Add(userCommunity);
            }

            // Add the community to the repository
            this.communityRepository.Add(community);

            // Save all the changes made.
            this.communityRepository.SaveChanges();

            return community.CommunityID;
        }

        /// <summary>
        /// Creates the new community in Layerscape with the given details passed in CommunitiesView instance.
        /// </summary>
        /// <param name="communityDetail">Details of the community</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Id of the community created. Returns -1 is creation is failed.</returns>
        public void UpdateCommunity(CommunityDetails communityDetail, long userID)
        {
            // Make sure communityDetails is not null
            this.CheckNotNull(() => new { communityDetails = communityDetail });

            Community community = this.communityRepository.GetItem((Community c) => c.CommunityID == communityDetail.ID && c.IsDeleted == false);

            // Make sure community is not null
            this.CheckNotNull(() => new { community });

            UserRole userRole = GetCommunityUserRole(community.CommunityID, userID);
            if (!CanEditDeleteCommunity(community, userID, userRole))
            {
                throw new HttpException(401, Resources.NoPermissionUpdateCommunityMessage);
            }

            // For deleted communities, do not updates the changes.
            if (community.IsDeleted.HasValue && !community.IsDeleted.Value)
            {
                // Do not let the user to change the Community as public in case if it is offensive.
                // This scenario might happen when the edit page is left open or cached and meantime Community is marked as offensive 
                // by the Site Admin.
                if (community.IsOffensive.HasValue && (bool)community.IsOffensive && communityDetail.AccessTypeID == (int)AccessType.Public)
                {
                    communityDetail.AccessTypeID = (int)AccessType.Private;
                    communityDetail.IsOffensive = true;
                }

                Mapper.Map(communityDetail, community);

                community.ModifiedByID = userID;
                community.ModifiedDatetime = DateTime.UtcNow;

                // 1. Add Thumbnail to blob
                if (communityDetail.Thumbnail != null)
                {
                    if (communityDetail.Thumbnail.AzureID != Guid.Empty && community.ThumbnailID != communityDetail.Thumbnail.AzureID)
                    {
                        if (MoveThumbnail(communityDetail.Thumbnail))
                        {
                            community.ThumbnailID = communityDetail.Thumbnail.AzureID;
                        }
                        else
                        {
                            community.ThumbnailID = Guid.Empty;
                        }
                    }
                    else if (communityDetail.Thumbnail.AzureID == Guid.Empty)
                    {
                        community.ThumbnailID = Guid.Empty;
                    }
                }

                // 2. Update Tag details. This will also take care of creating tags if they are not there in the Layerscape database.
                this.SetCommunityTags(communityDetail.Tags, community);

                // 3. Update User role details. Any change in parent, roles need to be updated.
                //      Even if there was no parent before, need to check if any parent is mentioned now.
                // Get the previous parent if any. Note that there will be only one parent.
                long previousParent = 0;
                if (community.CommunityRelation1.Count == 1)
                {
                    previousParent = Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation1, 0).ParentCommunityID;
                }

                if (communityDetail.ParentID != previousParent)
                {
                    this.userCommunitiesRepository.InheritParentRoles(community, communityDetail.ParentID);
                }

                // 4. Update Parent Community details in case if Parent is specified

                // TODO: Need to check if we can move the community.

                // Few things to be noted:
                // a) Obviously the count to be 0 or 1 always.
                // b) A community can be child of only once parent community or folder and hence only one CommunityRelation1
                if (community.CommunityRelation1.Count > 0 && Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation1, 0).ParentCommunityID != communityDetail.ParentID)
                {
                    community.CommunityRelation1.Clear();
                }

                if (communityDetail.ParentID > 0 && community.CommunityRelation1.Count == 0)
                {
                    // Add Parent Community details again
                    CommunityRelation communityRelation = new CommunityRelation();
                    communityRelation.ParentCommunityID = communityDetail.ParentID;
                    communityRelation.ChildCommunityID = communityDetail.ID;

                    // TODO: Need to rename the Data Model property CommunityRelation1 with a more meaningful name.
                    // Note that the relation to be added is to CommunityRelation1 since the current Community is Child.
                    // When the current community is parent, it's relation to be added in CommunityRelation.
                    community.CommunityRelation1.Add(communityRelation);
                }

                // Mark the Community as updated
                this.communityRepository.Update(community);

                // TODO: Need to check the concurrency scenarios.
                // Save all the changes made.
                this.communityRepository.SaveChanges();
            }
            else
            {
                // TODO: Need to throw exception informing user that the community is deleted.
            }
        }

        /// <summary>
        /// Deletes the specified community from the Earth database.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="isOffensive">Whether community is offensive or not.</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus DeleteCommunity(long communityID, long userID, bool isOffensive, OffensiveEntry offensiveDetails)
        {
            return this.DeleteCommunityRecursively(communityID, userID, isOffensive, offensiveDetails);
        }

        /// <summary>
        /// Deletes the specified community from the Earth database.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus DeleteCommunity(long communityID, long userID)
        {
            var details = new OffensiveEntry()
            {
                EntityID = communityID,
                ReviewerID = userID,
                Status = OffensiveStatusType.Deleted,
                Justification = "Deleted while deleting the Community."
            };
            return this.DeleteCommunityRecursively(communityID, userID, false, details);
        }

        /// <summary>
        /// Un-deletes the specified community in the Earth database so that it is again accessible in the site.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus UnDeleteOffensiveCommunity(long communityID, long userID)
        {
            OperationStatus status = null;

            try
            {
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    Community community = this.communityRepository.GetItem((Community c) => c.CommunityID == communityID);

                    // Make sure community exists
                    this.CheckNotNull(() => new { community });

                    this.UnDeleteCommunityContents(community, true);

                    this.communityRepository.Update(community);
                    this.communityRepository.SaveChanges();

                    // Create Success message if undelete is successful.
                    status = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Sets the given access type for the specified community.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="accessType">Access type of the community.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus SetCommunityAccessType(long communityID, long userID, AccessType accessType)
        {
            OperationStatus status = null;

            try
            {
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    Community community = this.communityRepository.GetItem((Community c) => c.CommunityID == communityID);

                    // Make sure community exists
                    this.CheckNotNull(() => new { community });

                    community.AccessTypeID = (int)accessType;

                    community.IsOffensive = (accessType == AccessType.Private);

                    var offensiveDetails = new OffensiveEntry()
                    {
                        EntityID = communityID,
                        ReviewerID = userID,
                        Status = OffensiveStatusType.Offensive
                    };

                    UpdateAllOffensiveCommunityEntry(communityID, offensiveDetails);

                    this.communityRepository.Update(community);
                    this.communityRepository.SaveChanges();

                    // Create Success message if set access type is successful.
                    status = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="communityID">Id of the current community which should not be returned</param>
        /// <param name="userID">User for which the parent communities/folders are being fetched</param>
        /// <returns>List of communities folders</returns>
        public IEnumerable<Community> GetParentCommunities(long communityID, long userID)
        {
            // Need to check if the user is site admin or not.
            UserRole currentUserRole = this.userRepository.GetUserRole(userID, null);
            return this.communityRepository.GetParentCommunities(userID, communityID, CommunityTypes.User, UserRole.Moderator, currentUserRole);
        }

        /// <summary>
        /// Get payload details for the community
        /// </summary>
        /// <param name="communityID">community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        public PayloadDetails GetPayload(long communityID, long? userID)
        {
            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
            var community = this.communityRepository.GetPayloadDetails(communityID);
            if (community != null)
            {
                payloadDetails.Name = community.Name;
                payloadDetails.Id = community.CommunityID.ToString(CultureInfo.InvariantCulture);
                payloadDetails.MSRCommunityId = community.CommunityID;
                payloadDetails.CommunityType = community.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.Folder);
                payloadDetails.Thumbnail = community.ThumbnailID.HasValue ? community.ThumbnailID.ToString() : null;

                // Set Child communities based on user permissions
                List<CommunityDetails> communityDetailsList = new List<CommunityDetails>();
                foreach (CommunityRelation child in community.CommunityRelation)
                {
                    var childCommunity = child.Community1;

                    try
                    {
                        var communityDetails = CreateCommunityDetails(childCommunity, userID, false);
                        //// TODO : Better way to move IsDeleted Check
                        if (communityDetails != null && childCommunity.IsDeleted == false)
                        {
                            communityDetailsList.Add(communityDetails);
                        }
                    }
                    catch (HttpException ex)
                    {
                        if (ex.GetHttpCode() == (int)HttpStatusCode.Unauthorized)
                        {
                            // In case if user is not authorized to access the community (private), unauthorized exception
                            // will be thrown which can be consumed and can continue with next communities.
                            continue;
                        }

                        throw;
                    }
                }

                payloadDetails.SetValuesFrom(communityDetailsList);

                // Set Child Content based on user permissions
                var contentDetailsList = GetContentDetailsFromContent(community.CommunityContents.Select(item => item.Content).Where(item => item.IsDeleted == false), userID);
                payloadDetails.SetValuesFrom(contentDetailsList);
            }

            return payloadDetails;
        }

        /// <summary>
        /// Get all tours in the community
        /// </summary>
        /// <param name="communityID">community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        public PayloadDetails GetAllTours(long communityID, long? userID)
        {
            var tourContents = this.communityRepository.GetAllTours(communityID);
            var contentDetailsList = GetContentDetailsFromContent(tourContents, userID);

            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.SetValuesFrom(contentDetailsList);

            return payloadDetails;
        }

        /// <summary>
        /// Get latest content in the community
        /// </summary>
        /// <param name="communityID">community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        public PayloadDetails GetLatestContent(long communityID, long? userID)
        {
            var latestContents = this.communityRepository.GetLatestContent(communityID, Constants.CommunityTourLatestFileDays);
            var contentDetailsList = GetContentDetailsFromContent(latestContents, userID);

            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.SetValuesFrom(contentDetailsList);

            return payloadDetails;
        }

        /// <summary>
        /// This function retrieves the communities to be shown at the root level for the user.
        /// </summary>
        /// <param name="userID">User identity</param>
        /// <returns>Payload details.</returns>
        public PayloadDetails GetRootCommunities(long userID)
        {
            UserRole currentUserRole = this.userRepository.GetUserRole(userID, null);
            IEnumerable<long> userCommunityIds = this.userRepository.GetUserCommunitiesForRole(userID, currentUserRole, false);
            Expression<Func<Community, bool>> condition = (Community c) => userCommunityIds.Contains(c.CommunityID);

            Func<Community, object> orderBy = (Community c) => c.ModifiedDatetime;
            var communities = this.communityRepository.GetItems(condition, orderBy, true);
            var communityDetailsList = new List<CommunityDetails>();
            foreach (Community community in communities)
            {
                var communityDetails = CreateCommunityDetails(community, userID, false);
                if (communityDetails != null)
                {
                    communityDetailsList.Add(communityDetails);
                }
            }

            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.Name = "My Communities";
            payloadDetails.Permission = Permission.Reader;
            payloadDetails.SetValuesFrom(communityDetailsList);

            return payloadDetails;
        }

        /// <summary>
        /// Gets the default community of the User.
        /// </summary>
        /// <param name="userID">user identification.</param>
        /// <returns>Default community details.</returns>
        public CommunityDetails GetDefaultCommunity(long userID)
        {
            CommunityDetails communityDetails = null;
            var community = this.communityRepository.GetItem(c => c.CommunityTypeID == (int)CommunityTypes.User && c.CreatedByID == userID);
            communityDetails = CreateCommunityDetails(community, userID, false);
            return communityDetails;
        }

        /// <summary>
        /// Retrieves the latest community IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        public IEnumerable<long> GetLatestCommunityIDs(int count)
        {
            return this.communityRepository.GetLatestCommunityIDs(count);
        }

        /// <summary>
        /// Invites the set of users specified in the invite request for the given community with the given role.
        /// </summary>
        /// <param name="inviteRequestItem">Invite request with details</param>
        /// <returns>Returns the collection of invite request send along with their tokens.</returns>
        public IEnumerable<InviteRequestItem> InvitePeople(InviteRequestItem inviteRequestItem)
        {
            // Make sure inviteRequest is not null.
            this.CheckNotNull(() => new { inviteRequestItem });
            IList<InviteRequestItem> invitedPeople = new List<InviteRequestItem>();
            UserRole userRole = this.userRepository.GetUserRole(inviteRequestItem.InvitedByID, inviteRequestItem.CommunityID);
            if (userRole >= UserRole.Moderator)
            {
                var community = this.communityRepository.GetItem(c => c.CommunityID == inviteRequestItem.CommunityID && c.IsDeleted == false);

                this.CheckNotNull(() => new { community });

                // Create the invite request content.
                InviteRequestContent inviteRequestContent = new InviteRequestContent();
                Mapper.Map(inviteRequestItem, inviteRequestContent);
                inviteRequestContent.InvitedDate = DateTime.UtcNow;

                foreach (var emailId in inviteRequestItem.EmailIdList)
                {
                    // For each user getting invited, add the invite request and associate it with the invite request content.
                    InviteRequest inviteRequest = new InviteRequest();
                    inviteRequest.EmailID = emailId;
                    inviteRequest.InviteRequestToken = Guid.NewGuid();
                    inviteRequest.Used = false;
                    inviteRequest.IsDeleted = false;

                    inviteRequestContent.InviteRequest.Add(inviteRequest);

                    InviteRequestItem invitedRequestItem = new InviteRequestItem();
                    Mapper.Map(inviteRequestItem, invitedRequestItem);
                    invitedRequestItem.EmailIdList.Add(emailId);
                    invitedRequestItem.InviteRequestToken = inviteRequest.InviteRequestToken;
                    invitedPeople.Add(invitedRequestItem);
                }

                // Add the invite request content to the community.
                community.InviteRequestContent.Add(inviteRequestContent);

                // Mark the community as updated.
                this.communityRepository.Update(community);

                // Save all the changes made.
                this.communityRepository.SaveChanges();
            }

            return invitedPeople;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sets the Community-Tags relations for given Tags. If the tags are not there in the DB, they will be added.
        /// </summary>
        /// <param name="tagsString">Comma separated tags string</param>
        /// <param name="community">Community to which tags to be related</param>
        internal void SetCommunityTags(string tagsString, Community community)
        {
            // Delete all existing tags which are not part of the new tags list.
            community.RemoveTags(tagsString);

            // Create Tags and relationships.
            if (!string.IsNullOrWhiteSpace(tagsString))
            {
                IEnumerable<string> tagsArray = tagsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
                if (tagsArray != null && tagsArray.Count() > 0)
                {
                    var notExistingTags = from tag in tagsArray
                                          where Enumerable.FirstOrDefault<CommunityTags>(community.CommunityTags, t => t.Tag.Name == tag) == null
                                          select tag;

                    foreach (string tag in notExistingTags)
                    {
                        Tag objTag = tagRepository.GetItem((Tag t) => t.Name == tag);

                        if (objTag == null)
                        {
                            objTag = new Tag();
                            objTag.Name = tag;
                        }

                        CommunityTags communityTag = new CommunityTags();
                        communityTag.Community = community;
                        communityTag.Tag = objTag;

                        community.CommunityTags.Add(communityTag);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified community from the Earth database.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="isOffensive">Community is offensive or not</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>True of the community is deleted. False otherwise.</returns>
        private OperationStatus DeleteCommunityRecursively(long communityID, long userID, bool isOffensive, OffensiveEntry offensiveDetails)
        {
            OperationStatus status = null;
            try
            {
                UserRole userRole = GetCommunityUserRole(communityID, userID);
                Community community = this.communityRepository.GetItem((Community c) => c.CommunityID == communityID && c.CommunityTypeID != (int)CommunityTypes.User && c.IsDeleted == false);

                if (community != null)
                {
                    if (CanEditDeleteCommunity(community, userID, userRole))
                    {
                        this.DeleteCommunityContents(community, userID, isOffensive, offensiveDetails);

                        community.IsOffensive = isOffensive;
                        community.DeletedByID = userID;

                        // Mark the Community as updated
                        this.communityRepository.Update(community);

                        // Save all the changes made.
                        this.communityRepository.SaveChanges();
                    }
                    else
                    {
                        status = OperationStatus.CreateFailureStatus("User does not have permission for deleting community.");
                    }
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(string.Format(CultureInfo.CurrentCulture, "Community with ID '{0}' was not found", communityID));
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Marks all the sub-communities/folder and contents of the community as un-deleted, only if they are not offensive.
        /// </summary>
        /// <param name="community">Community whose content and sub-communities/folder to be marked as un-deleted</param>
        /// <param name="updatePermissions">Update permissions for the community? Only for root community which is getting undeleted to be 
        /// updated for permissions</param>
        private void UnDeleteCommunityContents(Community community, bool updatePermissions)
        {
            if (community != null)
            {
                // NOTE: We Should NOT undelete the Community/Content if it is explicitly deleted by the user.
                // 1. Mark the community as not deleted.
                community.IsDeleted = false;

                // We need to mark the community as not offensive as the Admin is marking the community as undeleted.
                community.IsOffensive = false;

                // We need to mark the DeletedBy as null as we are Undoing the delete operation. 
                //  Also DeleteBy filed will be used to check if the user has explicitly deleted the Community.
                community.User2 = null;

                // 2. Check if parent community exists. If it exists and not deleted, continue to next step. If parent community
                //    exist and deleted, delete the parent relationship.
                if (community.CommunityRelation1.Count > 0)
                {
                    long parentCommunityID = Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation1, 0).Community.CommunityID;

                    if (Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation1, 0).Community.IsDeleted == true)
                    {
                        parentCommunityID = 0;
                        community.CommunityRelation1.Remove(Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation1, 0));
                    }

                    if (updatePermissions)
                    {
                        // 3. Update the permissions.
                        //      3.1 If there is not parent community is there or if parent community is deleted, mark all the 
                        //          permissions as direct, not inherited.
                        //      3.2 If the parent community is available, then check for the roles which are inherited from parent.
                        this.userCommunitiesRepository.InheritParentRoles(community, parentCommunityID);
                    }
                }

                // 4. Mark all the contents of the community as undeleted, only if they are not offensive.
                //      Also make sure we don't undelete contents which are explicitly deleted by the User. (DeletedBYID will be set explicitly if the content is deleted by user)
                for (int i = community.CommunityContents.Count - 1; i >= 0; i--)
                {
                    CommunityContents communityContent = Enumerable.ElementAt<CommunityContents>(community.CommunityContents, i);

                    // DeletedBy filed will be used to check if the user has explicitly deleted the Community. 
                    if (communityContent.Content.IsDeleted == true 
                        && communityContent.Content.DeletedByID == null 
                        && (!communityContent.Content.IsOffensive.HasValue || !(bool)communityContent.Content.IsOffensive))
                    {
                        communityContent.Content.IsDeleted = false;

                        // We need to mark the DeletedBy as null as we are Undoing the delete operation. 
                        //  Also DeleteBy filed will be used to check if the user has explicitly deleted the Community.
                        communityContent.Content.User2 = null;
                    }
                }

                // 5. Mark all the child communities and folders as undeleted. Note that current community is parent and
                //    all its relations with children will be there in CommunityRelation.
                for (int i = community.CommunityRelation.Count - 1; i >= 0; i--)
                {
                    CommunityRelation communityRelation = Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation, i);

                    // Incase if the same community is marked as its parent/child in DB directly, without this check delete call will go indefinitely.
                    if (communityRelation.Community1.IsDeleted == true 
                        && communityRelation.Community1.CommunityID != community.CommunityID 
                        && communityRelation.Community1.DeletedByID == null 
                        && (!communityRelation.Community1.IsOffensive.HasValue || !(bool)communityRelation.Community1.IsOffensive))
                    {
                        UnDeleteCommunityContents(communityRelation.Community1, false);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all the contents of the community, including its sub communities/folders and contents recursively.
        /// </summary>
        /// <param name="community">Community whose contents to be deleted</param>
        /// <param name="profileID">User Identity</param>
        private void DeleteCommunityContents(Community community, long profileID, bool isOffensive, OffensiveEntry offensiveDetails)
        {
            if (community != null)
            {
                community.DeletedDatetime = DateTime.UtcNow;
                community.IsDeleted = true;
                if (offensiveDetails.EntityID == community.CommunityID)
                {
                    community.IsOffensive = isOffensive;
                }

                // Update all the offensive entity entries if the community is being deleted.
                UpdateAllOffensiveCommunityEntry(community.CommunityID, offensiveDetails);

                // Mark all the contents of the community as deleted and also delete the relation entry from CommunityContents table.
                for (int i = community.CommunityContents.Count - 1; i >= 0; i--)
                {
                    CommunityContents communityContent = Enumerable.ElementAt<CommunityContents>(community.CommunityContents, i);

                    if (communityContent.Content.IsDeleted == false)
                    {
                        if (offensiveDetails.EntityID == community.CommunityID)
                        {
                            communityContent.Content.IsOffensive = isOffensive;
                        }

                        communityContent.Content.IsDeleted = true;
                        communityContent.Content.DeletedDatetime = DateTime.UtcNow;

                        // Update all the offensive entity entries if the community is being deleted.
                        UpdateAllOffensiveContentEntry(communityContent.Content.ContentID, offensiveDetails);
                    }
                }

                // Mark all the child communities and folders as deleted. Note that current community is parent and
                // all its relations with children will be there in CommunityRelation. Also deleting the relation entry from CommunityRelation table.
                for (int i = community.CommunityRelation.Count - 1; i >= 0; i--)
                {
                    CommunityRelation communityRelation = Enumerable.ElementAt<CommunityRelation>(community.CommunityRelation, i);

                    // Incase if the same community is marked as its parent/child in DB directly, without this check delete call will go indefinitely.
                    if (communityRelation.Community1.IsDeleted == false && communityRelation.Community1.CommunityID != community.CommunityID)
                    {
                        DeleteCommunityContents(communityRelation.Community1, profileID, isOffensive, offensiveDetails);
                    }
                }
            }
        }

        /// <summary>
        /// Moves thumbnail from temporary storage to thumbnail storage in azure.
        /// </summary>
        /// <param name="thumbnail">Details of the thumbnail</param>
        private bool MoveThumbnail(FileDetail thumbnail)
        {
            BlobDetails thumbnailBlob = new BlobDetails()
            {
                BlobID = thumbnail.AzureID.ToString()
            };

            return this.blobDataRepository.MoveThumbnail(thumbnailBlob);
        }

        /// <summary>
        /// Creates the community details for the given community. User id is used to check the user role
        /// on the community,based on that, CommunityDetails might be null in case is user is not having permission.
        /// </summary>
        /// <param name="community">Community for which community details has to be created</param>
        /// <param name="userID">Current user id</param>
        /// <param name="checkPendingRequest">Check for pending requests of the user</param>
        /// <returns>Community details instance</returns>
        private CommunityDetails CreateCommunityDetails(Community community, long? userID, bool checkPendingRequest)
        {
            CommunityDetails communityDetails = null;

            if (community != null)
            {
                Permission permission;
                UserRole userRole = GetCommunityUserRole(community.CommunityID, userID);

                if (!CanReadCommunity(userRole))
                {
                    throw new HttpException(401, Resources.NoPermissionReadCommunityMessage);
                }

                permission = userRole.GetPermission();

                // 1. For visitors (not assigned any roles for the community) who are logged in, need to find if any role request is pending approval.
                // 2. Pending approval is needed only for community details, not to be added in other places.
                if (checkPendingRequest &&
                        userRole == UserRole.Visitor &&
                        userID.HasValue &&
                        this.userRepository.PendingPermissionRequests(userID.Value, community.CommunityID))
                {
                    permission = Permission.PendingApproval;
                }

                communityDetails = new CommunityDetails(permission);

                // Some of the values which comes from complex objects need to be set through this method.
                communityDetails.SetValuesFrom(community);

                communityDetails.ViewCount = community.ViewCount.HasValue ? community.ViewCount.Value : 0;

                // Update parent details based on the permission.
                var parent = Enumerable.FirstOrDefault<CommunityRelation>(community.CommunityRelation1);
                if (parent != null && parent.Community != null)
                {
                    UserRole parentUserRole = GetCommunityUserRole(parent.Community.CommunityID, userID);

                    if (!CanReadCommunity(parentUserRole))
                    {
                        communityDetails.ParentName = string.Empty;
                        communityDetails.ParentID = -1;
                        communityDetails.ParentType = CommunityTypes.None;
                    }
                }
            }

            return communityDetails;
        }

        /// <summary>
        /// Creates the private community details for the given community. User id is used to check if the user is having any
        /// pending requests on the private community.
        /// </summary>
        /// <param name="community">Community for which private community details has to be created</param>
        /// <param name="userID">Current user id</param>
        /// <returns>Community details instance</returns>
        private CommunityDetails CreatePrivateCommunityDetails(Community community, long? userID)
        {
            CommunityDetails communityDetails = null;

            Permission permission = Permission.Visitor;

            // Check if already any pending approvals are there.
            if (userID.HasValue && this.userRepository.PendingPermissionRequests(userID.Value, community.CommunityID))
            {
                permission = Permission.PendingApproval;
            }

            communityDetails = new CommunityDetails(permission);
            communityDetails.ID = community.CommunityID;
            communityDetails.Name = community.Name;
            communityDetails.Description = community.Description;
            communityDetails.AccessTypeID = (int)AccessType.Private;
            communityDetails.CommunityType = (CommunityTypes)community.CommunityTypeID;

            // Set Thumbnail properties.
            var thumbnailDetail = new FileDetail();
            thumbnailDetail.AzureID = community.ThumbnailID.HasValue ? community.ThumbnailID.Value : Guid.Empty;
            communityDetails.Thumbnail = thumbnailDetail;

            return communityDetails;
        }

        /// <summary>
        /// Get ContentDetails from Content
        /// </summary>
        /// <param name="contents">contents list</param>
        /// <param name="userId">user identity</param>
        /// <returns>Content Details list</returns>
        private List<ContentDetails> GetContentDetailsFromContent(IEnumerable<Content> contents, long? userId)
        {
            // Set Child Content based on user permissions
            var contentDetailsList = new List<ContentDetails>();
            foreach (var childContent in contents)
            {
                var userRole = this.contentService.GetContentUserRole(childContent, userId);

                // For private contents, user's who have not assigned explicit permission will not have access.
                if (userRole != UserRole.None)
                {
                    var contentDetails = new ContentDetails(userRole.GetPermission());
                    contentDetails.SetValuesFrom(childContent);
                    contentDetailsList.Add(contentDetails);
                }
            }
            return contentDetailsList;
        }

        /// <summary>
        /// Increments the view count of the community identified by the Condition.
        /// </summary>
        /// <param name="community">Community for which view count has to be incremented</param>
        private void IncrementViewCount(Community community)
        {
            if (community != null)
            {
                // Increment view Count.
                community.ViewCount = community.ViewCount.HasValue ? community.ViewCount.Value + 1 : 1;
                this.communityRepository.Update(community);

                // Save changes to the database
                this.communityRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the all the entries for the given Community with all the details.
        /// </summary>
        /// <param name="communityID">ID of the current community.</param>
        /// <param name="details">Details provided.</param>
        /// <returns>True if Community was updated; otherwise false.</returns>
        private OperationStatus UpdateAllOffensiveCommunityEntry(long communityID, OffensiveEntry details)
        {
            OperationStatus status = null;
            try
            {
                var offensiveCommunities = this.offensiveCommunitiesRepository.GetItems(oc => oc.CommunityID == communityID && oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged, null, false);
                if (offensiveCommunities != null && offensiveCommunities.Count() > 0)
                {
                    foreach (var item in offensiveCommunities)
                    {
                        item.OffensiveStatusID = (int)details.Status;
                        item.Justification = details.Justification;

                        item.ReviewerID = details.ReviewerID;
                        item.ReviewerDatetime = DateTime.UtcNow;

                        this.offensiveCommunitiesRepository.Update(item);
                    }
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Updates the all the entries for the given Content with all the details.
        /// </summary>
        /// <param name="contentID">Content ID.</param>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        private OperationStatus UpdateAllOffensiveContentEntry(long contentID, OffensiveEntry details)
        {
            OperationStatus status = null;
            try
            {
                var offensiveContents = this.offensiveContentRepository.GetItems(oc => oc.ContentID == contentID && oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged, null, false);
                if (offensiveContents != null && offensiveContents.Count() > 0)
                {
                    foreach (var item in offensiveContents)
                    {
                        item.OffensiveStatusID = (int)details.Status;
                        item.Justification = details.Justification;

                        item.ReviewerID = details.ReviewerID;
                        item.ReviewerDatetime = DateTime.UtcNow;

                        this.offensiveContentRepository.Update(item);
                    }
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        #endregion Private Methods
    }
}

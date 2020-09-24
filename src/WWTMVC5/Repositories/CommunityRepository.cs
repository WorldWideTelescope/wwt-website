//-----------------------------------------------------------------------
// <copyright file="CommunityRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the community repository having methods for adding/retrieving/deleting/updating
    /// metadata about community from SQL Azure Layerscape database.
    /// </summary>
    public class CommunityRepository : RepositoryBase<Community>, ICommunityRepository
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommunityRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context.
        /// </param>
        public CommunityRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Gets the community specified by the community id. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="communityId">Id of the Community.</param>
        /// <returns>Community instance.</returns>
        public async Task<Community> GetCommunityAsync(long communityId)
        {
            var community = await EarthOnlineDbContext.Community
                .Where(item => item.CommunityID == communityId
                    && item.IsDeleted == false && item.CommunityTypeID != (int) CommunityTypes.User)
                .Include(c => c.AccessType)
                .Include(c => c.Category)
                .Include(c => c.CommunityRatings)
                .Include(c => c.CommunityRelation1)
                .Include(c => c.CommunityTags.Select(ct => ct.Tag))
                .Include(c => c.User).ToListAsync();

            return community.FirstOrDefault();
        }

        public Community GetCommunity(long communityId)
        {
            var community = EarthOnlineDbContext.Community
                .Where(item => item.CommunityID == communityId
                    && item.IsDeleted == false && item.CommunityTypeID != (int)CommunityTypes.User)
                .Include(c => c.AccessType)
                .Include(c => c.Category)
                .Include(c => c.CommunityRatings)
                .Include(c => c.CommunityRelation1)
                .Include(c => c.CommunityTags.Select(ct => ct.Tag))
                .Include(c => c.User)
                .FirstOrDefault();
            return community;
        }

        /// <summary>
        /// Retrieves the IDs of sub communities of a given community. This only retrieves the immediate children.
        /// </summary>
        /// <param name="communityId">
        /// ID of the community.
        /// </param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>
        /// Collection of IDS of sub communities.
        /// </returns>
        public IEnumerable<long> GetSubCommunityIDs(long communityId, long userId)
        {
            // TODO: Need to remove this once we have logic for loading objects from DBSet directly.
            IEnumerable<long> subCommunityIds = null;

            // Get the child communities for the given community first.
            var communitiesRelation = EarthOnlineDbContext.Set<CommunityRelation>();
            subCommunityIds = communitiesRelation.Where(relation => relation.ParentCommunityID == communityId).Select(relation => relation.ChildCommunityID);

            // Get the communities which are public. Get private communities only if the owner is current user.
            var result =
                EarthOnlineDbContext.Community.Where(community =>
                        subCommunityIds.Contains(community.CommunityID) && !(bool) community.IsDeleted &&
                        (community.AccessTypeID == (int) AccessType.Public ||
                         (EarthOnlineDbContext.User.FirstOrDefault(user => user.UserID == userId && user.UserTypeID == 1) != null ||
                          EarthOnlineDbContext.UserCommunities.FirstOrDefault(uc => uc.UserID == userId && uc.CommunityId == communityId &&
                                  uc.RoleID >= (int) UserRole.Reader) != null))).OrderByDescending(community => community.ModifiedDatetime).Select(community => community.CommunityID);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves the IDs of contents of a given community.
        /// </summary>
        /// <param name="communityId">
        /// ID of the community.
        /// </param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>
        /// Collection of IDs of contents.
        /// </returns>
        public IEnumerable<long> GetContentIDs(long communityId, long userId)
        {
            // TODO: Need to remove this once we have logic for loading objects from DBSet directly.
            IEnumerable<long> subContentIds = null;

            // Get the child contents for the given community/folder first.
            var communityContents = EarthOnlineDbContext.Set<CommunityContents>();
            subContentIds = communityContents.Where(relation => relation.CommunityID == communityId).Select(relation => relation.ContentID);

            var contentsView = EarthOnlineDbContext.Set<ContentsView>();

            // Get the contents which are public. 
            // Get private contents only if:
            //      1. If the user is site administrator.
            //      2. If the user is given explicit permission through roles.
            var result = contentsView
                            .Where(content => subContentIds.Contains(content.ContentID) && (content.AccessType == Resources.Public ||
                                        (EarthOnlineDbContext.User.FirstOrDefault(user => user.UserID == userId && user.UserTypeID == 1) != null ||
                                            EarthOnlineDbContext.UserCommunities.FirstOrDefault(uc => uc.UserID == userId && uc.CommunityId == communityId && uc.RoleID >= (int)UserRole.Reader) != null)))
                            .OrderByDescending(content => content.LastUpdatedDatetime)
                            .Select(content => content.ContentID);

            return result.ToList();
        }


        public async Task<IEnumerable<ContentsView>> GetContents(long communityId, long userId)
        {
            // TODO: Need to remove this once we have logic for loading objects from DBSet directly.

            // Get the child contents for the given community/folder first.
            var communityContents = EarthOnlineDbContext.Set<CommunityContents>();
            IEnumerable<long> subContentIds = communityContents.Where(relation => relation.CommunityID == communityId).Select(relation => relation.ContentID);

            var contentsView = EarthOnlineDbContext.Set<ContentsView>();

            // Get the contents which are public. 
            // Get private contents only if:
            //      1. If the user is site administrator.
            //      2. If the user is given explicit permission through roles.
            var result = await contentsView
                .Where(
                    content => subContentIds.Contains(content.ContentID) && (content.AccessType == Resources.Public ||
                        (EarthOnlineDbContext.User
                            .FirstOrDefault(user => user.UserID == userId &&
                                user.UserTypeID == 1) != null ||
                        EarthOnlineDbContext
                            .UserCommunities
                            .FirstOrDefault(uc => uc.UserID == userId &&
                                uc.CommunityId == communityId &&
                                uc.RoleID >= (int) UserRole.Reader) != null)))
                .OrderByDescending(content => content.LastUpdatedDatetime).ToListAsync();
                            
            return result;
        }

        /// <summary>
        /// Retrieves the payload details of a given community.
        /// </summary>
        /// <param name="communityId">
        /// ID of the community.
        /// </param>
        /// <returns>
        /// Payload details of a given community.
        /// </returns>
        public Community GetPayloadDetails(long communityId)
        {
            // Get Community details along with child content and child relationships
            var payloadDetails = EarthOnlineDbContext.Community.Where(item => item.CommunityID == communityId && item.IsDeleted == false)
                .Include(c => c.CommunityContents).Include(child => child.CommunityContents.Select(p => p.Content))
                .Include(c => c.CommunityRelation).Include(child => child.CommunityRelation.Select(p => p.Community1));

            return payloadDetails.FirstOrDefault();
        }

        /// <summary>
        /// Get All Tours for the community
        /// </summary>
        /// <param name="communityId">community ID</param>
        /// <returns>Tour content</returns>
        public IEnumerable<Content> GetAllTours(long communityId)
        {
            var childCommunities = GetAllChildrenCommunities(communityId);

            var payloadDetails = EarthOnlineDbContext.Community.Where(item => childCommunities.Contains(item.CommunityID) || (item.CommunityID == communityId))
                .Include(c => c.CommunityContents).Include(child => child.CommunityContents.Select(p => p.Content));

            var contents = payloadDetails.SelectMany(item => item.CommunityContents).Select(item => item.Content)
                .Where(item => item.Filename.EndsWith(Constants.TourFileExtension) && item.IsDeleted.Value.Equals(false));

            return contents.ToList();
        }

        /// <summary>
        /// Get latest content for the community
        /// </summary>
        /// <param name="communityId">community ID</param>
        /// <param name="daysToConsider">days to consider for latest</param>
        /// <returns>latest content</returns>
        public IEnumerable<Content> GetLatestContent(long communityId, int daysToConsider)
        {
            var childCommunities = GetAllChildrenCommunities(communityId);

            var payloadDetails = EarthOnlineDbContext.Community.Where(item => childCommunities.Contains(item.CommunityID) || (item.CommunityID == communityId))
                .Include(c => c.CommunityContents).Include(child => child.CommunityContents.Select(p => p.Content));

            var latestDateTime = DateTime.UtcNow.AddDays(-(daysToConsider));

            var contents = payloadDetails.SelectMany(item => item.CommunityContents)
                .Select(item => item.Content)
                .Where(item => item.ModifiedDatetime > latestDateTime && item.IsDeleted.Value.Equals(false))
                .OrderByDescending(item => item.ModifiedDatetime);

            return contents.ToList();
        }

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="userId">User for which the parent communities/folders are being fetched</param>
        /// <param name="currentCommunityId">Id of the current community which should not be returned</param>
        /// <param name="excludeCommunityType">Community type which needs to be excluded</param>
        /// <param name="userRoleOnParentCommunity">Specified user should have given user role or higher on the given community</param>
        /// <param name="currentUserRole">Current user role</param>
        /// <returns>List of communities folders</returns>
        public IEnumerable<Community> GetParentCommunities(
                long userId,
                long currentCommunityId,
                CommunityTypes excludeCommunityType,
                UserRole userRoleOnParentCommunity,
                UserRole currentUserRole)
        {
            Expression<Func<Community, bool>> condition = null;

            // TODO: How do we ensure that we consistently pass conditions like IsDeleted = false to all queries?
            if (currentUserRole == UserRole.SiteAdmin)
            {
                // For site administrator, need to get all the communities/folders.
                if (excludeCommunityType == CommunityTypes.None)
                {
                    // If parent communities are retrieved for Contents (when excludeCommunityType is None), need to send the "None" Community of site admin.
                    condition = (Community c) => (c.CommunityTypeID != (int)CommunityTypes.User || c.CreatedByID == userId) &&
                                                        !(bool)c.IsDeleted &&
                                                        c.CommunityID != currentCommunityId;
                }
                else
                {
                    if (currentCommunityId > 0)
                    {
                        var childCommunities = GetAllChildrenCommunities(currentCommunityId);

                        // If parent communities are retrieved for Communities (when excludeCommunityType is User), no need to send the "None" Community.
                        condition = (Community c) =>
                            !(bool)c.IsDeleted && 
                            c.CommunityTypeID != (int)excludeCommunityType && 
                            c.CommunityID != currentCommunityId && 
                            !childCommunities.Contains(c.CommunityID);
                    }
                    else
                    {
                        // If parent communities are retrieved for Communities (when excludeCommunityType is User), no need to send the "None" Community.
                        condition = (Community c) => !(bool)c.IsDeleted && c.CommunityTypeID != (int)excludeCommunityType && c.CommunityID != currentCommunityId;
                    }
                }
            }
            else
            {
                if (currentCommunityId > 0)
                {
                    var childCommunities = GetAllChildrenCommunities(currentCommunityId);

                    // User who is creating the community should be equal or higher role than the userRoleOnParentCommunity.
                    // 1. While creating communities/folder, User Community Type (Visitor folder) to be excluded.
                    // 2. While creating contents, User Community Type (Visitor folder) to be included.
                    condition = (Community c) => !(bool)c.IsDeleted &&
                        c.CommunityTypeID != (int)excludeCommunityType &&
                        c.CommunityID != currentCommunityId &&
                        c.UserCommunities.Where(u => u.UserID == userId && u.RoleID >= (int)userRoleOnParentCommunity).FirstOrDefault() != null && 
                        !childCommunities.Contains(c.CommunityID);
                }
                else
                {
                    // User who is creating the community should be equal or higher role than the userRoleOnParentCommunity.
                    // 1. While creating communities/folder, User Community Type (Visitor folder) to be excluded.
                    // 2. While creating contents, User Community Type (Visitor folder) to be included.
                    condition = (Community c) => !(bool)c.IsDeleted &&
                        c.CommunityTypeID != (int)excludeCommunityType &&
                        c.CommunityID != currentCommunityId &&
                        c.UserCommunities.Where(u => u.UserID == userId && u.RoleID >= (int)userRoleOnParentCommunity).FirstOrDefault() != null;
                }
            }

            Func<Community, object> orderBy = (Community c) => c.CommunityID;
            return EarthOnlineDbContext.Community.Where(condition).OrderBy(orderBy);
        }

        /// <summary>
        /// Retrieves the multiple instances of communities for the given IDs. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="communityIDs">
        /// Community IDs.
        /// </param>
        /// <returns>
        /// Collection of CommunitiesView.
        /// </returns>
        public IEnumerable<Community> GetItems(IEnumerable<long> communityIDs)
        {
            IEnumerable<Community> result = null;

            result = DbSet.Where(community => communityIDs.Contains(community.CommunityID))
                .Include(c => c.CommunityRatings)
                .Include(c => c.CommunityRelation1)
                .Include(c => c.CommunityTags.Select(ct => ct.Tag))
                .Include(c => c.User).OrderByDescending(community => community.ModifiedDatetime);

            return result.ToList();
        }

        /// <summary>
        /// Gets the access type for the given Community.
        /// </summary>
        /// <param name="communityId">Community for which access type has to be returned</param>
        /// <returns>Access type of the Community</returns>
        public string GetCommunityAccessType(long communityId)
        {
            var query = "SELECT dbo.GetCommunityAccessType(@communityID)";
            return EarthOnlineDbContext.Database.SqlQuery<string>(query, new SqlParameter("communityID", communityId)).FirstOrDefault();
        }

        /// <summary>
        /// Gets all the approvers details of a given community
        /// </summary>
        /// <param name="communityId">Identification of a community.</param>
        /// <returns>All approvers of a given community.</returns>
        public IEnumerable<User> GetApprovers(long communityId)
        {
            var approvers = from userCommunities in EarthOnlineDbContext.UserCommunities
                            where userCommunities.CommunityId == communityId && userCommunities.RoleID >= (int)UserRole.Moderator
                            select userCommunities.User;

            return approvers.ToList();
        }

        /// <summary>
        /// Gets all Contributors(including Moderators and Owners) of a given community of a given community
        /// </summary>
        /// <param name="communityId">Identification of a community.</param>
        /// <returns>All Contributors of a given community.</returns>
        public IEnumerable<User> GetContributors(long communityId)
        {
            var contributors = from userCommunities in EarthOnlineDbContext.UserCommunities
                               where userCommunities.CommunityId == communityId && userCommunities.RoleID >= (int)UserRole.Contributor
                               select userCommunities.User;

            return contributors.ToList();
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
            // Get the communities which are not deleted.
            var result = EarthOnlineDbContext.Community.Where(community => !(bool)community.IsDeleted &&
                                                                                community.CommunityTypeID != (int)CommunityTypes.User &&
                                                                                community.AccessTypeID == (int)AccessType.Public).OrderByDescending(community => community.CommunityID).Select(community => community.CommunityID)
                                .Take(count);

            return result.ToList();
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Retrieves the IDs of sub communities of a given community recursively for all children and grand children
        /// </summary>
        /// <param name="communityId">
        /// ID of the community.
        /// </param>
        /// <returns>
        /// Collection of IDS of sub communities.
        /// </returns>
        private IEnumerable<long> GetAllChildrenCommunities(long communityId)
        {
            IEnumerable<long> results = null;
            var communitiesRelation = EarthOnlineDbContext.Set<CommunityRelation>();

            results = communitiesRelation.Where(relation => relation.ParentCommunityID == communityId).Select(relation => relation.ChildCommunityID);

            // TODO : Optimize multiple calls going to DB
            foreach (var result in results)
            {
                results = results.Concat(GetAllChildrenCommunities(result));
            }

            return results.ToList();
        }

        #endregion Private methods
    }
}
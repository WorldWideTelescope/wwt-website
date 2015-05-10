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
        /// <param name="communityID">Id of the Community.</param>
        /// <returns>Community instance.</returns>
        public Community GetCommunity(long communityID)
        {
            var community = this.EarthOnlineDbContext.Community.Where<Community>(item => item.CommunityID == communityID
                                                                            && item.IsDeleted == false &&
                                                                            item.CommunityTypeID != (int)CommunityTypes.User)
                .Include(c => c.AccessType)
                .Include<Community, Category>(c => c.Category)
                .Include<Community, ICollection<CommunityRatings>>(c => c.CommunityRatings)
                .Include<Community, ICollection<CommunityRelation>>(c => c.CommunityRelation1)
                .Include(c => c.CommunityTags.Select<CommunityTags, Tag>(ct => ct.Tag))
                .Include<Community, User>(c => c.User)
                .FirstOrDefault();

            return community;
        }

        /// <summary>
        /// Retrieves the IDs of sub communities of a given community. This only retrieves the immediate children.
        /// </summary>
        /// <param name="communityID">
        /// ID of the community.
        /// </param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>
        /// Collection of IDS of sub communities.
        /// </returns>
        public IEnumerable<long> GetSubCommunityIDs(long communityID, long userId)
        {
            // TODO: Need to remove this once we have logic for loading objects from DBSet directly.
            IEnumerable<long> subCommunityIds = null;

            // Get the child communities for the given community first.
            DbSet<CommunityRelation> communitiesRelation = this.EarthOnlineDbContext.Set<CommunityRelation>();
            subCommunityIds = Queryable.Select<CommunityRelation, long>(Queryable.Where(communitiesRelation, relation => relation.ParentCommunityID == communityID), relation => relation.ChildCommunityID);

            // Get the communities which are public. Get private communities only if the owner is current user.
            var result = Queryable.Select<Community, long>(Queryable.OrderByDescending<Community, DateTime?>(Queryable.Where<Community>(this.EarthOnlineDbContext.Community, community => subCommunityIds.Contains(community.CommunityID) && !(bool)community.IsDeleted &&
                                                                                                                                                                                    (community.AccessTypeID == (int)AccessType.Public ||
                                                                                                                                                                                     (Queryable.Where<User>(this.EarthOnlineDbContext.User, user => user.UserID == userId && user.UserTypeID == 1).FirstOrDefault() != null ||
                                                                                                                                                                                      Queryable.Where<UserCommunities>(this.EarthOnlineDbContext.UserCommunities, uc => uc.UserID == userId && uc.CommunityId == communityID && uc.RoleID >= (int)UserRole.Reader).FirstOrDefault() != null))), community => community.ModifiedDatetime), community => community.CommunityID);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves the IDs of contents of a given community.
        /// </summary>
        /// <param name="communityID">
        /// ID of the community.
        /// </param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>
        /// Collection of IDs of contents.
        /// </returns>
        public IEnumerable<long> GetContentIDs(long communityID, long userId)
        {
            // TODO: Need to remove this once we have logic for loading objects from DBSet directly.
            IEnumerable<long> subContentIds = null;

            // Get the child contents for the given community/folder first.
            DbSet<CommunityContents> communityContents = this.EarthOnlineDbContext.Set<CommunityContents>();
            subContentIds = Queryable.Select<CommunityContents, long>(Queryable.Where(communityContents, relation => relation.CommunityID == communityID), relation => relation.ContentID);

            DbSet<ContentsView> contentsView = this.EarthOnlineDbContext.Set<ContentsView>();

            // Get the contents which are public. 
            // Get private contents only if:
            //      1. If the user is site administrator.
            //      2. If the user is given explicit permission through roles.
            var result = contentsView
                            .Where(content => subContentIds.Contains(content.ContentID) && (content.AccessType == Resources.Public ||
                                        (Queryable.Where<User>(this.EarthOnlineDbContext.User, user => user.UserID == userId && user.UserTypeID == 1).FirstOrDefault() != null ||
                                            Queryable.Where<UserCommunities>(this.EarthOnlineDbContext.UserCommunities, uc => uc.UserID == userId && uc.CommunityId == communityID && uc.RoleID >= (int)UserRole.Reader).FirstOrDefault() != null)))
                            .OrderByDescending(content => content.LastUpdatedDatetime)
                            .Select(content => content.ContentID);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves the payload details of a given community.
        /// </summary>
        /// <param name="communityID">
        /// ID of the community.
        /// </param>
        /// <returns>
        /// Payload details of a given community.
        /// </returns>
        public Community GetPayloadDetails(long communityID)
        {
            // Get Community details along with child content and child relationships
            var payloadDetails = Queryable.Where<Community>(this.EarthOnlineDbContext.Community, item => item.CommunityID == communityID && item.IsDeleted == false)
                .Include<Community, ICollection<CommunityContents>>(c => c.CommunityContents).Include(child => Enumerable.Select<CommunityContents, Content>(child.CommunityContents, p => p.Content))
                .Include<Community, ICollection<CommunityRelation>>(c => c.CommunityRelation).Include(child => Enumerable.Select<CommunityRelation, Community>(child.CommunityRelation, p => p.Community1));

            return payloadDetails.FirstOrDefault<Community>();
        }

        /// <summary>
        /// Get All Tours for the community
        /// </summary>
        /// <param name="communityID">community ID</param>
        /// <returns>Tour content</returns>
        public IEnumerable<Content> GetAllTours(long communityID)
        {
            var childCommunities = GetAllChildrenCommunities(communityID);

            var payloadDetails = Queryable.Where<Community>(this.EarthOnlineDbContext.Community, item => childCommunities.Contains(item.CommunityID) || (item.CommunityID == communityID))
                .Include<Community, ICollection<CommunityContents>>(c => c.CommunityContents).Include(child => Enumerable.Select<CommunityContents, Content>(child.CommunityContents, p => p.Content));

            var contents = Queryable.Select<CommunityContents, Content>(Queryable.SelectMany<Community, CommunityContents>(payloadDetails, item => item.CommunityContents), item => item.Content)
                .Where(item => item.Filename.EndsWith(Constants.TourFileExtension));

            return contents.ToList();
        }

        /// <summary>
        /// Get latest content for the community
        /// </summary>
        /// <param name="communityID">community ID</param>
        /// <param name="daysToConsider">days to consider for latest</param>
        /// <returns>latest content</returns>
        public IEnumerable<Content> GetLatestContent(long communityID, int daysToConsider)
        {
            var childCommunities = GetAllChildrenCommunities(communityID);

            var payloadDetails = Queryable.Where<Community>(this.EarthOnlineDbContext.Community, item => childCommunities.Contains(item.CommunityID) || (item.CommunityID == communityID))
                .Include<Community, ICollection<CommunityContents>>(c => c.CommunityContents).Include(child => Enumerable.Select<CommunityContents, Content>(child.CommunityContents, p => p.Content));

            DateTime latestDateTime = DateTime.UtcNow.AddDays(-(daysToConsider));

            var contents = Queryable.OrderByDescending<Content, DateTime?>(Queryable.Where(Queryable.Select<CommunityContents, Content>(Queryable.SelectMany<Community, CommunityContents>(payloadDetails, item => item.CommunityContents), item => item.Content), item => item.ModifiedDatetime > latestDateTime), item => item.ModifiedDatetime);

            return contents.ToList();
        }

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="userID">User for which the parent communities/folders are being fetched</param>
        /// <param name="currentCommunityId">Id of the current community which should not be returned</param>
        /// <param name="excludeCommunityType">Community type which needs to be excluded</param>
        /// <param name="userRoleOnParentCommunity">Specified user should have given user role or higher on the given community</param>
        /// <param name="currentUserRole">Current user role</param>
        /// <returns>List of communities folders</returns>
        public IEnumerable<Community> GetParentCommunities(
                long userID,
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
                    condition = (Community c) => (c.CommunityTypeID != (int)CommunityTypes.User || c.CreatedByID == userID) &&
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
                        Enumerable.Where<UserCommunities>(c.UserCommunities, u => u.UserID == userID && u.RoleID >= (int)userRoleOnParentCommunity).FirstOrDefault() != null && 
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
                        Enumerable.Where<UserCommunities>(c.UserCommunities, u => u.UserID == userID && u.RoleID >= (int)userRoleOnParentCommunity).FirstOrDefault() != null;
                }
            }

            Func<Community, object> orderBy = (Community c) => c.CommunityID;
            return Queryable.Where(this.EarthOnlineDbContext.Community, condition).OrderBy(orderBy);
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

            result = Queryable.OrderByDescending<Community, DateTime?>(Queryable.Where(this.DbSet, community => communityIDs.Contains(community.CommunityID))
                                    .Include<Community, ICollection<CommunityRatings>>(c => c.CommunityRatings)
                                    .Include<Community, ICollection<CommunityRelation>>(c => c.CommunityRelation1)
                                    .Include(c => Enumerable.Select<CommunityTags, Tag>(c.CommunityTags, ct => ct.Tag))
                                    .Include<Community, User>(c => c.User), community => community.ModifiedDatetime);

            return result.ToList();
        }

        /// <summary>
        /// Gets the access type for the given Community.
        /// </summary>
        /// <param name="communityID">Community for which access type has to be returned</param>
        /// <returns>Access type of the Community</returns>
        public string GetCommunityAccessType(long communityID)
        {
            string query = "SELECT dbo.GetCommunityAccessType(@communityID)";
            return this.EarthOnlineDbContext.Database.SqlQuery<string>(query, new SqlParameter("communityID", communityID)).FirstOrDefault();
        }

        /// <summary>
        /// Gets all the approvers details of a given community
        /// </summary>
        /// <param name="communityID">Identification of a community.</param>
        /// <returns>All approvers of a given community.</returns>
        public IEnumerable<User> GetApprovers(long communityID)
        {
            var approvers = from userCommunities in this.EarthOnlineDbContext.UserCommunities
                            where userCommunities.CommunityId == communityID && userCommunities.RoleID >= (int)UserRole.Moderator
                            select userCommunities.User;

            return Enumerable.ToList<User>(approvers);
        }

        /// <summary>
        /// Gets all Contributors(including Moderators and Owners) of a given community of a given community
        /// </summary>
        /// <param name="communityID">Identification of a community.</param>
        /// <returns>All Contributors of a given community.</returns>
        public IEnumerable<User> GetContributors(long communityID)
        {
            var contributors = from userCommunities in this.EarthOnlineDbContext.UserCommunities
                               where userCommunities.CommunityId == communityID && userCommunities.RoleID >= (int)UserRole.Contributor
                               select userCommunities.User;

            return Enumerable.ToList<User>(contributors);
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
            var result = Queryable.Select<Community, long>(Queryable.OrderByDescending<Community, long>(Queryable.Where<Community>(this.EarthOnlineDbContext.Community, community => !(bool)community.IsDeleted &&
                                                                                                                                                                                   community.CommunityTypeID != (int)CommunityTypes.User &&
                                                                                                                                                                                   community.AccessTypeID == (int)AccessType.Public), community => community.CommunityID), community => community.CommunityID)
                                .Take(count);

            return result.ToList();
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Retrieves the IDs of sub communities of a given community recursively for all children and grand children
        /// </summary>
        /// <param name="communityID">
        /// ID of the community.
        /// </param>
        /// <returns>
        /// Collection of IDS of sub communities.
        /// </returns>
        private IEnumerable<long> GetAllChildrenCommunities(long communityID)
        {
            IEnumerable<long> results = null;
            DbSet<CommunityRelation> communitiesRelation = this.EarthOnlineDbContext.Set<CommunityRelation>();

            results = Queryable.Select<CommunityRelation, long>(Queryable.Where(communitiesRelation, relation => relation.ParentCommunityID == communityID), relation => relation.ChildCommunityID);

            // TODO : Optimize multiple calls going to DB
            foreach (long result in results)
            {
                results = results.Concat(GetAllChildrenCommunities(result));
            }

            return results.ToList();
        }

        #endregion Private methods
    }
}
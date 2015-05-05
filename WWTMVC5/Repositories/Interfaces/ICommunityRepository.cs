//-----------------------------------------------------------------------
// <copyright file="ICommunityRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the rating repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommunityRepository : IRepositoryBase<Community>
    {
        /// <summary>
        /// Gets the community specified by the community id. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="communityID">Id of the Community.</param>
        /// <returns>Community instance.</returns>
        Community GetCommunity(long communityID);

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
        IEnumerable<long> GetSubCommunityIDs(long communityID, long userId);

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
        IEnumerable<long> GetContentIDs(long communityID, long userId);

        /// <summary>
        /// Retrieves the payload details of a given community.
        /// </summary>
        /// <param name="communityID">
        /// ID of the community.
        /// </param>
        /// <returns>
        /// Payload details of a given community.
        /// </returns>
        Community GetPayloadDetails(long communityID);

        /// <summary>
        /// Get All Tours for the community
        /// </summary>
        /// <param name="communityID">community ID</param>
        /// <returns>Tour content</returns>
        IEnumerable<Content> GetAllTours(long communityID);

        /// <summary>
        /// Get latest content for the community
        /// </summary>
        /// <param name="communityID">community ID</param>
        /// <param name="daysToConsider">days to consider for latest</param>
        /// <returns>latest content</returns>
        IEnumerable<Content> GetLatestContent(long communityID, int daysToConsider);

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
        IEnumerable<Community> GetParentCommunities(
                long userID,
                long currentCommunityId,
                CommunityTypes excludeCommunityType,
                UserRole userRoleOnParentCommunity,
                UserRole currentUserRole);

        /// <summary>
        /// Retrieves the multiple instances of communities for the given IDs. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="communityIDs">
        /// Community IDs.
        /// </param>
        /// <returns>
        /// Collection of CommunitiesView.
        /// </returns>
        IEnumerable<Community> GetItems(IEnumerable<long> communityIDs);

        /// <summary>
        /// Gets the access type for the given Community.
        /// </summary>
        /// <param name="communityID">Community for which access type has to be returned</param>
        /// <returns>Access type of the Community</returns>
        string GetCommunityAccessType(long communityID);

        /// <summary>
        /// Gets all the approvers details of a given community
        /// </summary>
        /// <param name="communityID">Identification of a community.</param>
        /// <returns>All approvers of a given community.</returns>
        IEnumerable<User> GetApprovers(long communityID);

        /// <summary>
        /// Gets all Contributors(including Moderators and Owners) of a given community of a given community
        /// </summary>
        /// <param name="communityID">Identification of a community.</param>
        /// <returns>All Contributors of a given community.</returns>
        IEnumerable<User> GetContributors(long communityID);

        /// <summary>
        /// Retrieves the latest community IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestCommunityIDs(int count);
    }
}
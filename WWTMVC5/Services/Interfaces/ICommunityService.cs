//-----------------------------------------------------------------------
// <copyright file="ICommunityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the community service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommunityService
    {
        /// <summary>
        /// Gets the community identified by community ID for EDIT purpose, means user must have required role for editing the community
        /// </summary>
        /// <param name="communityID">ID of the community which has to be retrieved</param>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <returns>Instance of community details</returns>
        CommunityDetails GetCommunityDetailsForEdit(long communityID, long userID);

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
        /// Instance of community details.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4 way of passing default parameters.")]
        CommunityDetails GetCommunityDetails(long communityID, long? userID, bool considerPrivateCommunity = false, bool updateReadCount = false);

        /// <summary>
        /// Creates the new community in Layerscape with the given details passed in CommunitiesView instance.
        /// </summary>
        /// <param name="communityDetail">Details of the community</param>
        /// <returns>Id of the community created. Returns -1 is creation is failed.</returns>
        long CreateCommunity(CommunityDetails communityDetail);

        /// <summary>
        /// Updates the given community in Layerscape with the given details passed in CommunitiesView instance.
        /// </summary>
        /// <param name="communityDetail">Details of the community</param>
        /// <param name="userID">User Identity</param>
        void UpdateCommunity(CommunityDetails communityDetail, long userID);
        
        /// <summary>
        /// Deletes the specified community from the Earth database.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="isOffensive">Whether community is offensive or not.</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus DeleteCommunity(long communityID, long userID, bool isOffensive, OffensiveEntry offensiveDetails);

        /// <summary>
        /// Deletes the specified community from the Earth database.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus DeleteCommunity(long communityID, long userID);

        /// <summary>
        /// Un-deletes the specified community in the Earth database so that it is again accessible in the site.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus UnDeleteOffensiveCommunity(long communityID, long userID);

        /// <summary>
        /// Sets the given access type for the specified community.
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="accessType">Access type of the community.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus SetCommunityAccessType(long communityID, long userID, AccessType accessType);

        /// <summary>
        /// Get payload details for the community
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        PayloadDetails GetPayload(long communityID, long? userID);

        /// <summary>
        /// Get all tours in the community
        /// </summary>
        /// <param name="communityID">community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        PayloadDetails GetAllTours(long communityID, long? userID);

        /// <summary>
        /// Get latest content in the community
        /// </summary>
        /// <param name="communityID">community Id</param>
        /// <param name="userID">user Identity</param>
        /// <returns>payload details</returns>
        PayloadDetails GetLatestContent(long communityID, long? userID);

        /// <summary>
        /// This function retrieves the root communities for the user.
        /// </summary>
        /// <param name="userID">User identity.</param>
        /// <returns>Payload details.</returns>
        PayloadDetails GetRootCommunities(long userID);
        

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="communityID">Id of the current community which should not be returned</param>
        /// <param name="userID">User for which the parent communities/folders are being fetched</param>
        /// <returns>List of communities folders</returns>
        IEnumerable<Community> GetParentCommunities(long communityID, long userID);

        /// <summary>
        /// Gets the default community of the User.
        /// </summary>
        /// <param name="userID">user identification.</param>
        /// <returns>Default community details.</returns>
        CommunityDetails GetDefaultCommunity(long userID);

        /// <summary>
        /// Retrieves the latest community IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestCommunityIDs(int count);

        /// <summary>
        /// Invites the set of users specified in the invite request for the given community with the given role.
        /// </summary>
        /// <param name="inviteRequestItem">Invite request with details</param>
        /// <returns>Returns the collection of invite request send along with their tokens.</returns>
        IEnumerable<InviteRequestItem> InvitePeople(InviteRequestItem inviteRequestItem);
    }
}

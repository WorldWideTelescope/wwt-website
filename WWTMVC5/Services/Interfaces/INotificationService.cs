//-----------------------------------------------------------------------
// <copyright file="INotificationService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the notification service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notify the moderators and owners about the an offensive community/content.
        /// </summary>
        /// <param name="notification">Offensive entity details</param>
        void NotifyJoinCommunity(JoinCommunityRequest notification);

        /// <summary>
        /// Notify the moderators and owners about the an offensive community/content.
        /// </summary>
        /// <param name="notification">Offensive entity details</param>
        void NotifyFlagged(FlaggedRequest notification);

        /// <summary>
        /// Notify the moderators and owners about a new comment on an entity.
        /// </summary>
        /// <param name="notification">Comment details</param>
        void NotifyEntityComment(EntityCommentRequest notification);

        /// <summary>
        /// Notify the moderators and owners about a approval/denial on a user request.
        /// </summary>
        /// <param name="notification">Permission details</param>
        void NotifyModeratorPermissionStatus(ModeratorPermissionStatusRequest notification);

        /// <summary>
        /// Notify the the user about his removal from an community.
        /// </summary>
        /// <param name="notification">Community details</param>
        void NotifyRemoveUser(RemoveUserRequest notification);

        /// <summary>
        /// Notify the about his change in the permissions.
        /// </summary>
        /// <param name="notification">Permission details</param>
        void NotifyUserPermissionChangedStatus(UserPermissionChangedRequest notification);

        /// <summary>
        /// Notify the user about his permission approval/denial.
        /// </summary>
        /// <param name="notification">Permission details</param>
        void NotifyUserRequestPermissionStatus(UserPermissionStatusRequest notification);

        /// <summary>
        /// Notify the user about the invitation sent for joining a community.
        /// </summary>
        /// <param name="notification">Invitation request details</param>
        void NotifyCommunityInviteRequest(NotifyInviteRequest notification);
        
        /// <summary>
        /// Notify the user about the entity has been deleted by the Admin.
        /// </summary>
        /// <param name="notification">Entity Admin Delete Request details</param>
        void NotifyEntityDeleteRequest(EntityAdminActionRequest notification);

        /// <summary>
        /// Notify the user about the new entity.
        /// </summary>
        /// <param name="notification">New Entity Request details</param>
        void NotifyNewEntityRequest(NewEntityRequest notification);

        /// <summary>
        /// Notify the user about the new Content.
        /// </summary>
        /// <param name="contentDetails">Content details</param>
        /// <param name="server">Server details.</param>
        void NotifyNewEntityRequest(ContentDetails contentDetails, string server);

        /// <summary>
        /// Notify the user about the new community.
        /// </summary>
        /// <param name="communityDetails">Community details</param>
        /// <param name="server">Server details.</param>
        void NotifyNewEntityRequest(CommunityDetails communityDetails, string server);
        
        /// <summary>
        /// Notify the user about the new User.
        /// </summary>
        /// <param name="profileDetails">User details</param>
        /// <param name="server">Server details.</param>
        void NotifyNewEntityRequest(ProfileDetails profileDetails, string server);
    }
}

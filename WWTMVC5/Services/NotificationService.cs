//-----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Globalization;
using WWTMVC5.Models;
using WWTMVC5;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    public class NotificationService : INotificationService
    {
        private IQueueRepository queueRepository;

        /// <summary>
        /// Initializes a new instance of the NotificationService class.
        /// </summary>
        /// <param name="queueRepository">
        /// Instance of queue repository.
        /// </param>
        public NotificationService(IQueueRepository queueRepository)
        {
            this.queueRepository = queueRepository;
        }

        /// <summary>
        /// Notify the moderators and owners about the an offensive community/content.
        /// </summary>
        /// <param name="notification">Offensive entity details</param>
        public void NotifyJoinCommunity(JoinCommunityRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the moderators and owners about the an offensive community/content.
        /// </summary>
        /// <param name="notification">Offensive entity details</param>
        public void NotifyFlagged(FlaggedRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the moderators and owners about a new comment on an entity.
        /// </summary>
        /// <param name="notification">Comment details</param>
        public void NotifyEntityComment(EntityCommentRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the moderators and owners about a approval/denial on a user request.
        /// </summary>
        /// <param name="notification">Permission details</param>
        public void NotifyModeratorPermissionStatus(ModeratorPermissionStatusRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the the user about his removal from an community.
        /// </summary>
        /// <param name="notification">Community details</param>
        public void NotifyRemoveUser(RemoveUserRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the about his change in the permissions.
        /// </summary>
        /// <param name="notification">Permission details</param>
        public void NotifyUserPermissionChangedStatus(UserPermissionChangedRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the user about his permission approval/denial.
        /// </summary>
        /// <param name="notification">Permission details</param>
        public void NotifyUserRequestPermissionStatus(UserPermissionStatusRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the user about the invitation sent for joining a community.
        /// </summary>
        /// <param name="notification">Invitation request details</param>
        public void NotifyCommunityInviteRequest(NotifyInviteRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the user about the entity has been deleted by the Admin.
        /// </summary>
        /// <param name="notification">Entity Admin Delete Request details</param>
        public void NotifyEntityDeleteRequest(EntityAdminActionRequest notification)
        {
            SendMail(notification);
        }

        /// <summary>
        /// Notify the user about the new entity.
        /// </summary>
        /// <param name="notification">New Entity Request details</param>
        public void NotifyNewEntityRequest(NewEntityRequest notification)
        {
            if (Constants.CanSendNewEntityMail)
            {
                SendMail(notification);
            }
        }

        /// <summary>
        /// Notify the user about the new Content.
        /// </summary>
        /// <param name="contentDetails">Content details</param>
        /// <param name="server">Server details.</param>
        public void NotifyNewEntityRequest(ContentDetails contentDetails, string server)
        {
            if (Constants.CanSendNewEntityMail)
            {
                SendNewContentMail(contentDetails, server);
            }
        }

        /// <summary>
        /// Notify the user about the new community.
        /// </summary>
        /// <param name="communityDetails">Community details</param>
        /// <param name="server">Server details.</param>
        public void NotifyNewEntityRequest(CommunityDetails communityDetails, string server)
        {
            if (Constants.CanSendNewEntityMail)
            {
                SendNewCommunityMail(communityDetails, server);
            }
        }

        /// <summary>
        /// Notify the user about the new User.
        /// </summary>
        /// <param name="profileDetails">User details</param>
        /// <param name="server">Server details.</param>
        public void NotifyNewEntityRequest(ProfileDetails profileDetails, string server)
        {
            if (Constants.CanSendNewEntityMail)
            {
                SendNewUserMail(profileDetails, server);
            }
        }

        private void SendMail(object notification)
        {
            CloudQueueMessage message = this.queueRepository.Pack(notification);
            this.queueRepository.NotificationQueue.AddMessage(message);
        }

        /// <summary>
        /// Send New content mail.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendNewContentMail(ContentDetails contentDetails, string server)
        {
            try
            {
                // Send Mail.
                var request = new NewEntityRequest
                {
                    EntityType = EntityType.Content,
                    EntityID = contentDetails.ID,
                    EntityName = contentDetails.Name,
                    EntityLink =
                        string.Format(CultureInfo.InvariantCulture, "{0}Content/Index/{1}", server, contentDetails.ID),
                    UserID = contentDetails.CreatedByID,
                    UserLink =
                        string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", server,
                            contentDetails.CreatedByID)
                };

                SendMail(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send New community mail.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendNewCommunityMail(CommunityDetails communityDetails, string server)
        {
            try
            {
                // Send Mail.
                NewEntityRequest request = new NewEntityRequest();
                request.EntityType = communityDetails.CommunityType == CommunityTypes.Community ? EntityType.Community : EntityType.Folder;
                request.EntityID = communityDetails.ID;
                request.EntityName = communityDetails.Name;
                request.EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", server, communityDetails.ID);
                request.UserID = communityDetails.CreatedByID;
                request.UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", server, communityDetails.CreatedByID);

                SendMail(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send New User mail.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendNewUserMail(ProfileDetails profileDetails, string server)
        {
            try
            {
                // Send Mail.
                NewEntityRequest request = new NewEntityRequest();
                request.EntityType = EntityType.User;
                request.EntityID = profileDetails.ID;
                request.EntityName = profileDetails.FirstName + " " + profileDetails.LastName;
                request.EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", server, profileDetails.ID);

                SendMail(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }
    }
}
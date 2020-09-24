//-----------------------------------------------------------------------
// <copyright file="NotifyInviteRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Notification Invite Request item representing invite request for a community
    /// </summary>
    [Serializable]
    public class NotifyInviteRequest
    {
        /// <summary>
        /// Gets or sets the ID of the community
        /// </summary>
        public long CommunityID { get; set; }

        /// <summary>
        /// Gets or sets the name of the community
        /// </summary>
        public string CommunityName { get; set; }

        /// <summary>
        /// Gets or sets the link for the community
        /// </summary>
        public string CommunityLink { get; set; }

        /// <summary>
        /// Gets or sets the invited user's email id
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the invite link
        /// </summary>
        public string InviteLink { get; set; }
    }
}
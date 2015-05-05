//-----------------------------------------------------------------------
// <copyright file="InviteRequestItem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Invite Request item representing invite request for a community
    /// </summary>
    [Serializable]
    public class InviteRequestItem
    {
        /// <summary>
        /// Initializes a new instance of the InviteRequestItem class.
        /// </summary>
        public InviteRequestItem()
        {
            this.EmailIdList = new Collection<string>();
        }

        /// <summary>
        /// Gets or sets the ID of the Invite Request ID
        /// </summary>
        public int InviteRequestID { get; set; }

        /// <summary>
        /// Gets or sets the Token of the Invite Request
        /// </summary>
        public Guid InviteRequestToken { get; set; }

        /// <summary>
        /// Gets or sets the invited user's email ids
        /// </summary>
        public Collection<string> EmailIdList { get; set; }

        /// <summary>
        /// Gets or sets the ID of the community
        /// </summary>
        public long CommunityID { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the user id who sent the invite
        /// </summary>
        public long InvitedByID { get; set; }

        /// <summary>
        /// Gets or sets the user name who sent the invite
        /// </summary>
        public string InvitedBy { get; set; }

        /// <summary>
        /// Gets or sets the invited date time 
        /// </summary>
        public DateTime InvitedDate { get; set; }
    }
}
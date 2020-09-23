//-----------------------------------------------------------------------
// <copyright file="InviteRequestDetailsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View of community invite requests
    /// </summary>
    public class InviteRequestDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the Invite Request ID
        /// </summary>
        public int InviteRequestID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the community
        /// </summary>
        public long CommunityID { get; set; }

        /// <summary>
        /// Gets or sets the invited user's email id
        /// </summary>
        public string EmailId { get; set; }

        /// <summary>
        /// Gets or sets the Role of the invite
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the Subject of the invite
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Body of the invite
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the invited date time
        /// </summary>
        public DateTime InvitedDate { get; set; }
    }
}

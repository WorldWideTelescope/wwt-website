//-----------------------------------------------------------------------
// <copyright file="InviteRequestInputViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the View Model which gets input for sending the invites.
    /// </summary>
    public class InviteRequestInputViewModel
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
        /// Gets or sets the current user's permission on the community.
        /// </summary>
        public Permission UserPermission { get; set; }

        /// <summary>
        /// Gets or sets the Subject of the invite
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Body of the invite
        /// </summary>
        public string Body { get; set; }
    }
}
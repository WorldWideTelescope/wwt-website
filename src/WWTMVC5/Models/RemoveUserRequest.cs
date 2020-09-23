//-----------------------------------------------------------------------
// <copyright file="RemoveUserRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a RemoveUser request.
    /// </summary>
    [Serializable]
    public class RemoveUserRequest
    {
        /// <summary>
        /// Gets or sets the name of the User.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the identification of the User.
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets the link of the User.
        /// </summary>
        public string UserLink { get; set; }

        /// <summary>
        /// Gets or sets the name of the Community.
        /// </summary>
        public string CommunityName { get; set; }

        /// <summary>
        /// Gets or sets the identification of the community.
        /// </summary>
        public long CommunityID { get; set; }

        /// <summary>
        /// Gets or sets the link for the community.
        /// </summary>
        public string CommunityLink { get; set; }
    }
}
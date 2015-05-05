//-----------------------------------------------------------------------
// <copyright file="JoinCommunityRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a ModeratorPermissionStatusRequest.
    /// </summary>
    [Serializable]
    public class JoinCommunityRequest
    {
        /// <summary>
        /// Gets or sets the name of the requestor.
        /// </summary>
        public string RequestorName { get; set; }

        /// <summary>
        /// Gets or sets the identification of the requestor.
        /// </summary>
        public long RequestorID { get; set; }

        /// <summary>
        /// Gets or sets the link of the requestor.
        /// </summary>
        public string RequestorLink { get; set; }

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

        /// <summary>
        /// Gets or sets the Permission Requested.
        /// </summary>
        public string PermissionRequested { get; set; }
    }
}
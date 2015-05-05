//-----------------------------------------------------------------------
// <copyright file="CommunityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a Community.
    /// </summary>
    [Serializable]
    public class CommunityDetails : EntityDetails
    {
        /// <summary>
        /// Initializes a new instance of the CommunityDetails class.
        /// </summary>
        public CommunityDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommunityDetails class.
        /// </summary>
        /// <param name="permission">Permissions of the user on the community</param>
        public CommunityDetails(Permission permission)
            : base(permission)
        {
        }

        /// <summary>
        /// Gets or sets the view count
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// Gets or sets the type of community.
        /// </summary>
        public CommunityTypes CommunityType { get; set; }
    }
}
//-----------------------------------------------------------------------
// <copyright file="PermissionDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about the user permissions.
    /// </summary>
    [Serializable]
    public class PermissionDetails
    {
        /// <summary>
        /// Initializes a new instance of the PermissionDetails class.
        /// </summary>
        public PermissionDetails()
        {
            PermissionItemList = new List<PermissionItem>();
        }

        /// <summary>
        /// Gets or sets the current users permission. This is needed at permission details level when roles/requests are fetched for 
        /// a single community. User permission on the community for which role/requests are fetched will be returned with this even if
        /// are no role/request available.
        /// </summary>
        public Permission CurrentUserPermission { get; set; }

        /// <summary>
        /// Gets or sets the user roles on a community
        /// </summary>
        public IList<PermissionItem> PermissionItemList { get; set; }
    }
}

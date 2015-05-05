//-----------------------------------------------------------------------
// <copyright file="PermissionItem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Permission item representing user role on community
    /// </summary>
    [Serializable]
    public class PermissionItem
    {
        /// <summary>
        /// Gets or sets the ID of the user
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the community
        /// </summary>
        public long CommunityID { get; set; }

        /// <summary>
        /// Gets or sets the name of the community
        /// </summary>
        public string CommunityName { get; set; }

        /// <summary>
        /// Gets or sets the requestor comments
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the requested date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the Role requested
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the role of the user who is accessing the permission request or user role.
        /// </summary>
        public UserRole CurrentUserRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request is accepted or not.
        /// </summary>
        public bool? Approved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permissions are inherited or not.
        /// </summary>
        public bool IsInherited { get; set; }
    }
}
//-----------------------------------------------------------------------
// <copyright file="PermissionDetailsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View of community permission
    /// </summary>
    public class PermissionDetailsViewModel : PermissionBaseViewModel
    {
        /// <summary>
        /// Gets or sets the requestor comments
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the requested date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the Community Id
        /// </summary>
        public long CommunityId { get; set; }

        /// <summary>
        /// Gets or sets the name of Community
        /// </summary>
        public string CommunityName { get; set; }

        /// <summary>
        /// Gets or sets the role of the user who is accessing the permission request or user role.
        /// </summary>
        public UserRole CurrentUserRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permissions are inherited or not.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edit link should be shown or not.
        /// </summary>
        public bool CanShowEditLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the delete link should be shown or not.
        /// </summary>
        public bool CanShowDeleteLink { get; set; }
    }
}

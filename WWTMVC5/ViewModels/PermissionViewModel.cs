//-----------------------------------------------------------------------
// <copyright file="PermissionViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View of Community Permission
    /// </summary>
    public class PermissionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the PermissionViewModel class.
        /// </summary>
        public PermissionViewModel(
                Permission currentUserPermission,
                IList<PermissionDetailsViewModel> permissionItemList,
                PageDetails paginationDetails,
                PermissionsTab selectedPermissionsTab)
        {
            this.CurrentUserPermission = currentUserPermission;
            this.PermissionItemList = permissionItemList;
            this.PaginationDetails = paginationDetails;
            this.SelectedPermissionsTab = selectedPermissionsTab;
        }

        /// <summary>
        /// Gets the current users permission
        /// </summary>
        public Permission CurrentUserPermission { get; private set; }

        /// <summary>
        /// Gets the user roles on a community
        /// </summary>
        public IList<PermissionDetailsViewModel> PermissionItemList { get; private set; }

        /// <summary>
        /// Gets or sets the page details for the permissions.
        /// </summary>
        public PageDetails PaginationDetails { get; set; }

        /// <summary>
        /// Gets or sets the tab to be shown (User roles/Requests).
        /// </summary>
        public PermissionsTab SelectedPermissionsTab { get; set; }
    }
}
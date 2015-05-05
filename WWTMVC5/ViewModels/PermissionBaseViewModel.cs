//-----------------------------------------------------------------------
// <copyright file="PermissionBaseViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View of community permission
    /// </summary>
    public class PermissionBaseViewModel
    {
        /// <summary>
        /// Gets or sets the community / content id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name community / content
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Permission
        /// </summary>
        public UserRole Role { get; set; }
    }
}

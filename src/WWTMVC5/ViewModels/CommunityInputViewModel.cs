//-----------------------------------------------------------------------
// <copyright file="CommunityInputViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the View Model which gets input for creating or updating a community.
    /// </summary>
    public class CommunityInputViewModel : EntityInputBaseViewModel
    {
        /// <summary>
        /// Gets or sets the type of community.
        /// </summary>
        public CommunityTypes CommunityType { get; set; }
    }
}
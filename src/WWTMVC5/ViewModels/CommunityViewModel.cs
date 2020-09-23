//-----------------------------------------------------------------------
// <copyright file="CommunityViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the view for a Community with all details
    /// to be shown in community details page.
    /// </summary>
    public class CommunityViewModel : EntityBaseDetailsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the CommunityViewModel class.
        /// </summary>
        public CommunityViewModel()
        {
            Entity = EntityType.Community;
        }

        /// <summary>
        /// Gets or sets the total number of members in the community
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Gets or sets the view count
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// Gets or sets the share url of social icons
        /// </summary>
        public ShareViewModel ShareUrl { get; set; }
    }
}

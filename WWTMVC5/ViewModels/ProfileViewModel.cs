//-----------------------------------------------------------------------
// <copyright file="ProfileViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web;
using System.Web.Mvc;
using WWTMVC5.Extensions;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View for edit profile
    /// to be shown in profile page.
    /// </summary>
    public class ProfileViewModel
    {
        /// <summary>
        /// Private member for Affiliation property
        /// </summary>
        private string affiliation;

        /// <summary>
        /// Private member for AboutProfile property
        /// </summary>
        private string aboutProfile;

        /// <summary>
        /// Gets or sets the User Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// Gets or sets the User Name of the profile
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the profile description of the user
        /// </summary>
        [AllowHtml]
        public string AboutProfile
        {
            get
            {
                return aboutProfile;
            }
            set
            {
                aboutProfile = HttpContext.Current.Server.HtmlDecode(value);
            }
        }

        /// <summary>
        /// Gets or sets the affiliation of the user
        /// </summary>
        public string Affiliation
        {
            get
            {
                return affiliation;
            }
            set
            {
                affiliation = value.DecodeAndReplace();
            }
        }

        /// <summary>
        /// Gets or sets the Link for the profile photo
        /// </summary>
        public string ProfilePhotoLink { get; set; }

        /// <summary>
        /// Gets or sets the Total Storage
        /// </summary>
        public string TotalStorage { get; set; }

        /// <summary>
        /// Gets or sets the Used Storage
        /// </summary>
        public string UsedStorage { get; set; }

        /// <summary>
        /// Gets or sets the Available Storage
        /// </summary>
        public string AvailableStorage { get; set; }

        /// <summary>
        /// Gets or sets the Percentage Used Storage
        /// </summary>
        public string PercentageUsedStorage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current profile is of the current logged-in user .
        /// </summary>
        public bool IsCurrentUser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user has subscribed to notifications or not.
        /// </summary>
        public bool IsSubscribed { get; set; }
    }
}
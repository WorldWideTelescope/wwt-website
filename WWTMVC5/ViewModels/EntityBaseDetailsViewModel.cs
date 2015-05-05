//-----------------------------------------------------------------------
// <copyright file="EntityBaseDetailsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model that will be inherited by either a content or a community view model with all the details
    /// to be shown in community/content details.
    /// </summary>
    public abstract class EntityBaseDetailsViewModel : EntityViewModel
    {
        /// <summary>
        /// Private member for Description property
        /// </summary>
        private string description;

        /// <summary>
        /// Gets or sets the description of content/community
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = HttpContext.Current.Server.HtmlDecode(value);
            }
        }

        /// <summary>
        /// Gets or sets the Last updated date of content/community
        /// </summary>
        public string LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the detail page url of the content
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is offensive or not.
        /// </summary>
        public bool IsOffensive { get; set; }
    }
}

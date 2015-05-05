//-----------------------------------------------------------------------
// <copyright file="ShareViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the Share view
    /// to be shown in content page. Since every content/community will have one unique url to be shared
    /// in the social network, the controller will fill the below url for using in
    /// the view.
    /// </summary>
    public class ShareViewModel
    {
        /// <summary>
        /// Gets or sets Google share url
        /// </summary>
        public Uri GooglePlusUrl { get; set; }

        /// <summary>
        /// Gets or sets Twitter share url
        /// </summary>
        public Uri TwitterUrl { get; set; }

        /// <summary>
        /// Gets or sets linked link share url
        /// </summary>
        public Uri LinkedUrl { get; set; }

        /// <summary>
        /// Gets or sets facebook share url
        /// </summary>
        public Uri FacebookUrl { get; set; }

        /// <summary>
        /// Gets or sets MailTo url
        /// </summary>
        public Uri MailToUrl { get; set; }
    }
}
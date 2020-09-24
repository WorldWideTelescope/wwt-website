//-----------------------------------------------------------------------
// <copyright file="CommentItemViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the comments
    /// to be shown in content/community page.
    /// </summary>
    public class CommentItemViewModel
    {
        /// <summary>
        /// Gets or sets comment id.
        /// </summary>
        public long CommentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the community/content
        /// </summary>
        public string CommentedBy { get; set; }

        /// <summary>
        /// Gets or sets Commented by id.
        /// </summary>
        public long CommentedByID { get; set; }

        /// <summary>
        /// Gets or sets user photo
        /// </summary>
        public string UserImageLink { get; set; }

        /// <summary>
        /// Gets or sets the posted date of the comment
        /// </summary>
        public string PostedDate { get; set; }

        /// <summary>
        /// Gets or sets the comment text
        /// </summary>
        [AllowHtml]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether delete controls to be shown or not.
        /// </summary>
        public bool CanDelete { get; set; }
    }
}
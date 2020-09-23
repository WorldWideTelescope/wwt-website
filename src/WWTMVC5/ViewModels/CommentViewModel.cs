//-----------------------------------------------------------------------
// <copyright file="CommentViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the comments
    /// to be shown in content/community page.
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// Initializes a new instance of the CommentViewModel class.
        /// </summary>
        public CommentViewModel()
        {
            this.Comments = new List<CommentItemViewModel>();
        }

        /// <summary>
        /// Gets the collection of comments for a community/content
        /// </summary>
        public IList<CommentItemViewModel> Comments { get; private set; }

        /// <summary>
        /// Gets or sets the details about the page.
        /// </summary>
        public PageDetails PaginationDetails { get; set; }
    }
}
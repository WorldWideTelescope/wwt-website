//-----------------------------------------------------------------------
// <copyright file="CommentDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a Comment.
    /// </summary>
    [Serializable]
    public class CommentDetails
    {
        /// <summary>
        /// Gets or sets comment id.
        /// </summary>
        public long CommentID { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets Parent id.
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets Commented date time.
        /// </summary>
        public DateTime CommentedDatetime { get; set; }

        /// <summary>
        /// Gets or sets produced by.
        /// </summary>
        public string CommentedBy { get; set; }

        /// <summary>
        /// Gets or sets Commented by id.
        /// </summary>
        public long CommentedByID { get; set; }

        /// <summary>
        /// Gets or sets the profile picture id of the commented user
        /// </summary>
        public Guid? CommentedByPictureID { get; set; }
   }
}
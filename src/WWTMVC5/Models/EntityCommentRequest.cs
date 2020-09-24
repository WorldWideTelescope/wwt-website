//-----------------------------------------------------------------------
// <copyright file="EntityCommentRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a the request when a user comments on a community/content.
    /// </summary>
    [Serializable]
    public class EntityCommentRequest
    {
        /// <summary>
        /// Gets or sets the name of the type on which the User has commented. 
        /// This can be Community/Content
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the identification of the user.
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the link of the user.
        /// </summary>
        public string UserLink { get; set; }

        /// <summary>
        /// Gets or sets the identification of the Community/Content.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the link for the Community/Content.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the comments from the user.
        /// </summary>
        public string UserComments { get; set; }
    }
}
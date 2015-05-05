//-----------------------------------------------------------------------
// <copyright file="FlaggedRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a the request when a user flags a community/content.
    /// </summary>
    [Serializable]
    public class FlaggedRequest
    {
        /// <summary>
        /// Gets or sets the name of the type on which the User has flagged. 
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
        /// Gets or sets the identification of the Parent Community.
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the link for the Community/Content.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the flagged on date.
        /// </summary>
        public DateTime FlaggedOn { get; set; }

        /// <summary>
        /// Gets or sets the Flagged option.
        /// </summary>
        public string FlaggedAs { get; set; }

        /// <summary>
        /// Gets or sets the comments from the user.
        /// </summary>
        public string UserComments { get; set; }
    }
}
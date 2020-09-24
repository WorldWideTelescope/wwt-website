//-----------------------------------------------------------------------
// <copyright file="StaticContentDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about static content.
    /// </summary>
    [Serializable]
    public class StaticContentDetails
    {
        /// <summary>
        /// Gets or sets Type id.
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets Modified by id.
        /// </summary>
        public long ModifiedByID { get; set; }
   }
}
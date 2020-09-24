//-----------------------------------------------------------------------
// <copyright file="RatingDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a Rating.
    /// </summary>
    [Serializable]
    public class RatingDetails
    {
        /// <summary>
        /// Gets or sets the Rating.
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets Parent id.
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets rated by.
        /// </summary>
        public string RatedBy { get; set; }

        /// <summary>
        /// Gets or sets rated by id.
        /// </summary>
        public long RatedByID { get; set; }
   }
}
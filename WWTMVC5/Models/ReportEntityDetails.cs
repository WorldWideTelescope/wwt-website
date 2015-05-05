//-----------------------------------------------------------------------
// <copyright file="ReportEntityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a ReportEntity.
    /// </summary>
    [Serializable]
    public class ReportEntityDetails
    {
        /// <summary>
        /// Gets or sets ReportEntity id.
        /// </summary>
        public long ReportEntityID { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets Parent id.
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets Reported date time.
        /// </summary>
        public DateTime ReportedDatetime { get; set; }

        /// <summary>
        /// Gets or sets Reported by.
        /// </summary>
        public string ReportedBy { get; set; }

        /// <summary>
        /// Gets or sets Reported by id.
        /// </summary>
        public long ReportedByID { get; set; }

        /// <summary>
        /// Gets or sets the Report Entity type.
        /// </summary>
        public ReportEntityType ReportEntityType { get; set; }

        /// <summary>
        /// Gets or sets the offensive status.
        /// </summary>
        public OffensiveStatusType Status { get; set; }
    }
}
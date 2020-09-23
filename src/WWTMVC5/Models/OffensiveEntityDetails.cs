//-----------------------------------------------------------------------
// <copyright file="OffensiveEntityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a OffensiveEntity.
    /// </summary>
    [DataContract(Namespace = "")]
    public class OffensiveEntityDetails
    {
        /// <summary>
        /// Gets or sets Entry id.
        /// </summary>
        [DataMember]
        public long EntryID { get; set; }

        /// <summary>
        /// Gets or sets id of the Entity.
        /// </summary>
        [DataMember]
        public long EntityID { get; set; }

        /// <summary>
        /// Gets or sets Name of the Entity.
        /// </summary>
        [DataMember]
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets Name of the Entity.
        /// </summary>
        [DataMember]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets Reported date time.
        /// </summary>
        [DataMember]
        public DateTime ReportedDatetime { get; set; }

        /// <summary>
        /// Gets or sets Reported by.
        /// </summary>
        [DataMember]
        public string ReportedBy { get; set; }

        /// <summary>
        /// Gets or sets Reported by id.
        /// </summary>
        [DataMember]
        public long ReportedByID { get; set; }

        /// <summary>
        /// Gets or sets the Report Entity type.
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [DataMember]
        public string Comment { get; set; }
    }
}
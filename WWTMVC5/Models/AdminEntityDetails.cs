//-----------------------------------------------------------------------
// <copyright file="AdminEntityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a AdminEntity.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AdminEntityDetails
    {
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
        public DateTime ModifiedDatetime { get; set; }

        /// <summary>
        /// Gets or sets the Report Entity type.
        /// </summary>
        [DataMember]
        public string DistributedBy { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets category ID.
        /// </summary>
        [DataMember]
        public CategoryType Category { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [DataMember]
        public string Visibility { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is deleted or not.
        /// </summary>
        [DataMember]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is offensive or not.
        /// </summary>
        [DataMember]
        public bool IsOffensive { get; set; }

        /// <summary>
        /// Gets or sets the Created by.
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the Created by ID.
        /// </summary>
        [DataMember]
        public long CreatedByID { get; set; }
    }
}
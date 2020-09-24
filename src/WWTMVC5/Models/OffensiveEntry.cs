//-----------------------------------------------------------------------
// <copyright file="OffensiveEntry.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WWTMVC5.Models
{
    public class OffensiveEntry
    {
        /// <summary>
        /// Gets or sets the entry ID of the entity
        /// </summary>
        public long EntryID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the entity
        /// </summary>
        public long EntityID { get; set; }

        /// <summary>
        /// Gets or sets the entity type of the 
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the reviewer id.
        /// </summary>
        public long ReviewerID { get; set; }

        /// <summary>
        /// Gets or sets the offensive status
        /// </summary>
        public OffensiveStatusType Status { get; set; }

        /// <summary>
        /// Gets or sets the justification
        /// </summary>
        public string Justification { get; set; }
    }
}
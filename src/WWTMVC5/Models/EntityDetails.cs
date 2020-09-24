//-----------------------------------------------------------------------
// <copyright file="EntityDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a base Entity (Community or Content).
    /// </summary>
    [Serializable]
    public class EntityDetails
    {
        /// <summary>
        /// Initializes a new instance of the EntityDetails class.
        /// </summary>
        public EntityDetails()
        {
            this.UserPermission = Permission.Reader;

            // Tags should not be null so that empty strings can be passed as value for partial views.
            this.Tags = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the EntityDetails class.
        /// </summary>
        /// <param name="permission">Permissions of the user on the entity</param>
        public EntityDetails(Permission permission)
        {
            this.UserPermission = permission;

            // Tags should not be null so that empty strings can be passed as value for partial views.
            this.Tags = string.Empty;
        }

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the title of the content.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description of the content.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Parent name.
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Gets or sets Parent id.
        /// </summary>
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent.
        /// </summary>
        public CommunityTypes ParentType { get; set; }

        /// <summary>
        /// Gets or sets access type id.
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets Thumbnail details.
        /// </summary>
        public FileDetail Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether content is featured or not.
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether entity is deleted or not.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets category ID.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets access type name.
        /// </summary>
        public string AccessTypeName { get; set; }

        /// <summary>
        /// Gets or sets access type id.
        /// </summary>
        public int AccessTypeID { get; set; }

        /// <summary>
        /// Gets or sets average rating.
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Gets or sets number of people who have rated.
        /// </summary>
        public int RatedPeople { get; set; }

        /// <summary>
        /// Gets or sets tags in comma separated value.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets created date time.
        /// </summary>
        public DateTime? CreatedDatetime { get; set; }

        /// <summary>
        /// Gets or sets Created by id.
        /// </summary>
        public long CreatedByID { get; set; }

        /// <summary>
        /// Gets or sets produced by.
        /// </summary>
        public string ProducedBy { get; set; }

        /// <summary>
        /// Gets or sets DistributedBy.
        /// </summary>
        public string DistributedBy { get; set; }

        /// <summary>
        /// Gets or sets last updated time.
        /// </summary>
        public DateTime? LastUpdatedDatetime { get; set; }

        /// <summary>
        /// Gets the current user's permission on this entity.
        /// </summary>
        public Permission UserPermission { get; private set; }
        
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is offensive or not.
        /// </summary>
        public bool IsOffensive { get; set; }
   }
}
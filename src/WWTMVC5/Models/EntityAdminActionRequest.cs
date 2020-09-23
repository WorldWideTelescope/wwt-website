//-----------------------------------------------------------------------
// <copyright file="EntityAdminActionRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Admin has performed an action on the entity item.
    /// </summary>
    [Serializable]
    public class EntityAdminActionRequest
    {
        /// <summary>
        /// Gets or sets the ID of the Entity
        /// </summary>
        public long EntityID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Entity
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the Link of the Entity
        /// </summary>
        public string EntityLink { get; set; }

        /// <summary>
        /// Gets or sets the identification of the Admin.
        /// </summary>
        public long AdminID { get; set; }

        /// <summary>
        /// Gets or sets the action performed by Admin.
        /// </summary>
        public AdminActions Action { get; set; }
    }
}
//-----------------------------------------------------------------------
// <copyright file="NewEntityRequest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// New entity has been added to the Layerscape site.
    /// </summary>
    [Serializable]
    public class NewEntityRequest
    {
        /// <summary>
        /// Gets or sets the Type of the Entity
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Entity
        /// </summary>
        public long EntityID { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the Link of the Entity
        /// </summary>
        public string EntityLink { get; set; }

        /// <summary>
        /// Gets or sets the identification of the user.
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets the link of the user.
        /// </summary>
        public string UserLink { get; set; }
    }
}
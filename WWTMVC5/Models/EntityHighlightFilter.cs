//-----------------------------------------------------------------------
// <copyright file="EntityHighlightFilter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about the filters needed while retrieving collection of entities.
    /// </summary>
    [Serializable]
    public class EntityHighlightFilter
    {
        /// <summary>
        /// Initializes a new instance of the EntityHighlightFilter class.
        /// </summary>
        /// <param name="highlightType">Highlight type for the contents (Featured/Latest/Popular/Related?)</param>
        /// <param name="categoryType">Category type from which contents to be picked from</param>
        /// <param name="entityId">Content Id for which the relevant contents are picked</param>
        public EntityHighlightFilter(HighlightType highlightType, CategoryType categoryType, long? entityId)
        {
            this.HighlightType = highlightType;
            this.CategoryType = categoryType;
            this.EntityId = entityId;
            this.ContentType = ContentTypes.All;
        }

        public EntityHighlightFilter(HighlightType highlightType, CategoryType categoryType, long? entityId,
            ContentTypes contentType)
        {
            this.HighlightType = highlightType;
            this.CategoryType = categoryType;
            this.EntityId = entityId;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets or sets the highlight type (Featured/Latest/Popular)
        /// </summary>
        public HighlightType HighlightType { get; set; }

        /// <summary>
        /// Gets or sets the category type of the entities
        /// </summary>
        public CategoryType CategoryType { get; set; }

        /// <summary>
        /// Gets or sets the content type of the entities
        /// </summary>
        public ContentTypes ContentType { get; set; }

        /// <summary>
        /// Gets or sets the entity id in case if the entities are retrieved for any specific entities.
        /// </summary>
        public long? EntityId { get; set; }

        /// <summary>
        /// Gets or sets the user who is getting the highlight entities
        /// </summary>
        public long UserID { get; set; }
    }
}
//-----------------------------------------------------------------------
// <copyright file="FacetCategory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Facet category
    /// </summary>
    /// <typeparam name="Model">The type of model this category will use</typeparam>
    public class FacetCategory<Model>
    {
        /// <summary>
        /// Initializes a new instance of the FacetCategory class
        /// </summary>
        public FacetCategory()
        {
        }

        /// <summary>
        /// Gets or sets the name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the category
        /// </summary>
        public FacetType FacetDataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to this is a filterable category
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is filterable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Leave as null to ignore this</remarks>
        public bool? IsFilterVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the meta data
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is meta data visible; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Leave as null to ignore this</remarks>
        public bool? IsMetaDataVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the word wheel
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is word wheel; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Leave as null to ignore this</remarks>
        public bool? IsWordWheelVisible { get; set; }

        /// <summary>
        /// Gets or sets the Action delegate
        /// </summary>
        public Func<Model, string> Action { get; set; }

        /// <summary>
        /// Gets or sets the Href delegate
        /// </summary>
        public Func<Model, string> Href { get; set; }
    }
}
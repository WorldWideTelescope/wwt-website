//-----------------------------------------------------------------------
// <copyright file="FacetType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WWTMVC5.Models
{
    /// <summary>
    /// Facet type
    /// </summary>
    public enum FacetType
    {
        /// <summary>
        /// the facet is a string
        /// </summary>
        String,

        /// <summary>
        /// the facet is a long string
        /// </summary>
        LongString,

        /// <summary>
        /// the facet is a number
        /// </summary>
        Number,

        /// <summary>
        /// the facet is a date time
        /// </summary>
        /// <remarks>
        /// use <c>value.ToString("s")</c> to export as ISO
        /// </remarks>
        DateTime,

        /// <summary>
        /// The fact is a link
        /// </summary>
        Link
    }
}
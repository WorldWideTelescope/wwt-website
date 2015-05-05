//-----------------------------------------------------------------------
// <copyright file="DeepZoomItem.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WWTMVC5.Models
{
    /// <summary>
    /// DeepZoomItem class
    /// </summary>
    public class DeepZoomItem
    {
        /// <summary>
        /// Initializes a new instance of the DeepZoomItem class
        /// </summary>
        public DeepZoomItem()
        {
        }

        /// <summary>
        /// Gets or sets the N value of the item
        /// </summary>
        public string N { get; set; }

        /// <summary>
        /// Gets or sets the Id of the item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the image source of the item
        /// </summary>
        public string Source { get; set; }
    }
}
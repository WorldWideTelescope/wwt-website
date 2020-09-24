//-----------------------------------------------------------------------
// <copyright file="TopCategoryViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View for top category
    /// to be shown in home page.
    /// </summary>
    public class TopCategoryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the TopCategoryViewModel class.
        /// </summary>
        public TopCategoryViewModel()
        {
            Communities = new List<EntityViewModel>(2);
        }

        /// <summary>
        /// Gets or sets the Category of the top category item
        /// </summary>
        public CategoryType Category { get; set; }

        /// <summary>
        /// Gets or sets content detail
        /// </summary>
        public EntityViewModel Content { get; set; }

        /// <summary>
        /// Gets or sets top rated community
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This cannot be a read only parameter")]
        public IList<EntityViewModel> Communities { get; set; }
    }
}
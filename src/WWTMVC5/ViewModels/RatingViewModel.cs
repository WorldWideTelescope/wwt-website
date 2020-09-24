//-----------------------------------------------------------------------
// <copyright file="RatingViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the view for a Rating with all details
    /// to be shown in comment partial view.
    /// </summary>
    public class RatingViewModel
    {
        /// <summary>
        /// Gets or sets the rated value
        /// </summary>
        public int RatingValue { get; set; }

        /// <summary>
        /// Gets or sets the Either community id or content id
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the Either Community or Content
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the number of people those have rated the entity
        /// </summary>
        public int RatedPeople { get; set; }

        /// <summary>
        /// Gets or sets the  average rating e.g.(3+4+2+3)/4 = 3 (Total ratings for the entity id / number of people those are rated)
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show people rated or not
        /// </summary>
        public bool ShowRatedPeople { get; set; }
    }
}
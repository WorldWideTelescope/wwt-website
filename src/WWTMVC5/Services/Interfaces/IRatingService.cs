//-----------------------------------------------------------------------
// <copyright file="IRatingService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the rating service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IRatingService
    {
        /// <summary>
        /// Update Community Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <returns>True if the rating was updated successfully; Otherwise false.</returns>
        bool UpdateCommunityRating(RatingDetails rating);

        /// <summary>
        /// Update Content Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <returns>True if the rating was updated successfully; Otherwise false.</returns>
        bool UpdateContentRating(RatingDetails rating);
    }
}
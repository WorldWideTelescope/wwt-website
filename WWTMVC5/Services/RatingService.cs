//-----------------------------------------------------------------------
// <copyright file="RatingService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Rating Service having methods for retrieving Rating
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class RatingService : IRatingService
    {
        #region Member variables
        /// <summary>
        /// Instance of ContentRatings repository
        /// </summary>
        private IRepositoryBase<ContentRatings> contentRatingRepository;

        /// <summary>
        /// Instance of CommunityRatings repository
        /// </summary>
        private IRepositoryBase<CommunityRatings> communityRatingRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RatingService class.
        /// </summary>
        /// <param name="contentRatingsRepository">Instance of ContentRatings repository</param>
        /// <param name="communityRatingRepository">Instance of CommunityRatings repository</param>
        public RatingService(IRepositoryBase<ContentRatings> contentRatingsRepository, IRepositoryBase<CommunityRatings> communityRatingRepository)
        {
            this.contentRatingRepository = contentRatingsRepository;
            this.communityRatingRepository = communityRatingRepository;
        }

        #endregion

        #region Pubic Methods

        /// <summary>
        /// Update Community Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <returns>True if the rating was updated successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool UpdateCommunityRating(RatingDetails rating)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { rating });

            try
            {
                CommunityRatings communityRatings = this.communityRatingRepository
                    .GetItem(r => (r.CommunityID == rating.ParentID && r.RatedByID == rating.RatedByID));

                if (communityRatings != null)
                {
                    UpdateCommunityRating(rating, communityRatings);
                }
                else
                {
                    CreateCommunityRating(rating);
                }

                return true;
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return false;
        }

        /// <summary>
        /// Update Content Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <returns>True if the rating was updated successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool UpdateContentRating(RatingDetails rating)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { rating });

            try
            {
                ContentRatings contentRatings = this.contentRatingRepository
                    .GetItem(r => (r.ContentID == rating.ParentID && r.RatingByID == rating.RatedByID));

                if (contentRatings != null)
                {
                    UpdateContentRating(rating, contentRatings);
                }
                else
                {
                    CreateContentRating(rating);
                }

                return true;
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create community Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        private void CreateCommunityRating(RatingDetails rating)
        {
            CommunityRatings communityRatings = new CommunityRatings();
            Mapper.Map(rating, communityRatings);

            communityRatings.ModifiedDatetime = communityRatings.CreatedDatetime = DateTime.UtcNow;

            this.communityRatingRepository.Add(communityRatings);
            this.communityRatingRepository.SaveChanges();
        }

        /// <summary>
        /// Create Content Rating
        /// </summary>
        /// <param name="rating">Rating details.</param>
        private void CreateContentRating(RatingDetails rating)
        {
            ContentRatings contentRating = new ContentRatings();
            Mapper.Map(rating, contentRating);

            contentRating.ModifiedDatetime = contentRating.CreatedDatetime = DateTime.UtcNow;

            this.contentRatingRepository.Add(contentRating);
            this.contentRatingRepository.SaveChanges();
        }

        /// <summary>
        /// Update Community Rating.
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <param name="communityRatings">Instance of CommunityRatings.</param>
        private void UpdateCommunityRating(RatingDetails rating, CommunityRatings communityRatings)
        {
            communityRatings.Rating = rating.Rating;
            communityRatings.ModifiedDatetime = DateTime.UtcNow;

            this.communityRatingRepository.Update(communityRatings);
            this.communityRatingRepository.SaveChanges();
        }

        /// <summary>
        /// Update Content Rating.
        /// </summary>
        /// <param name="rating">Rating details.</param>
        /// <param name="contentRatings">Instance of ContentRatings.</param>
        private void UpdateContentRating(RatingDetails rating, ContentRatings contentRatings)
        {
            contentRatings.Rating = rating.Rating;
            contentRatings.ModifiedDatetime = DateTime.UtcNow;

            this.contentRatingRepository.Update(contentRatings);
            this.contentRatingRepository.SaveChanges();
        }

        #endregion
    }
}
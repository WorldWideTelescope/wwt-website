//-----------------------------------------------------------------------
// <copyright file="RatingController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the rating partial view request which makes request to repository and gets/publishes the
    /// required data about rating and pushes them to the View.
    /// </summary>
    public class RatingController : ControllerBase
    {  
        #region Member variables

        /// <summary>
        /// Instance of Rating Service
        /// </summary>
        private IRatingService _ratingService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RatingController class.
        /// </summary>
        /// <param name="ratingService">Instance of Rating Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public RatingController(IRatingService ratingService, IProfileService profileService)
            : base(profileService)
        {
            _ratingService = ratingService;
        }
        
        #endregion

        #region Action Methods

        [HttpPost]
        [Route("RatingConversion/{contentId}/{rating}/{userId}")]
        public JsonResult ConverstRatings(int contentId, int rating, int userId)
        {
            // Sending data to business logic
            var ratingDetails = new RatingDetails()
            {
                Rating = rating,
                RatedByID = userId,
                ParentID = contentId
            };

            var status = false;
            
            status = _ratingService.UpdateContentRating(ratingDetails);
            

            return Json(status);
        }

        /// <summary>
        /// Saves the rating
        /// </summary>
        /// <param name="contentId">The content being rated</param>
        /// <param name="rating">1-5 rating</param>
        /// <param name="type">entity type</param>
        /// <returns>success bool</returns>
        [HttpPost]
        [Route("Rating/{contentId}/{rating}/{type=Content}")]
        public bool Rate(int contentId, int rating, EntityType type)
        {
            // Sending data to business logic
            var ratingDetails = new RatingDetails()
            {
                Rating = rating,
                RatedByID = CurrentUserId,
                ParentID = contentId
            };

            var status = false;
            switch (type)
            {
                case EntityType.All:
                    break;
                case EntityType.Community:
                case EntityType.Folder:
                    status = _ratingService.UpdateCommunityRating(ratingDetails);
                    break;
                case EntityType.Content:
                    status = _ratingService.UpdateContentRating(ratingDetails);
                    break;
            }

            return status;
        }


        #endregion
    }
}
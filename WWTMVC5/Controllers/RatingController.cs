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
            this._ratingService = ratingService;
        }
        
        #endregion

        #region Action Methods

        /// <summary>
        /// Saves the rating
        /// </summary>
        /// <param name="ratingData">The model that has some properties</param>
        /// <returns>JSON with status</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]// remove salt= http://stackoverflow.com/questions/10851283/antiforgerytoken-deprecated-in-asp-net-mvc-4-rc
        public string Save([Bind(Exclude = "RatedPeople,AverageRating,ShowRatedCount")]RatingViewModel ratingData)
        {
            // Check input
            this.CheckNotNull(() => new { ratingData });

            // Sending data to business logic
            RatingDetails rating = new RatingDetails()
            {
                Rating = ratingData.RatingValue,
                RatedByID = CurrentUserId,
                ParentID = ratingData.EntityId
            };

            bool status = false;
            switch (ratingData.EntityType)
            {
                case EntityType.All:
                    break;
                case EntityType.Community:
                case EntityType.Folder:
                    status = this._ratingService.UpdateCommunityRating(rating);
                    break;
                case EntityType.Content:
                    status = this._ratingService.UpdateContentRating(rating);
                    break;
                default:
                    break;
            }

            return (status ? "Success" : "Failure");
        }

        /// <summary>
        /// It renders the rating partial view
        /// </summary>
        /// <param name="ratingModel">The model that has some properties</param>
        [ChildActionOnly]
        public void Render([Bind(Exclude = "RatingValue")]RatingViewModel ratingModel)
        {
            try
            {
                PartialView("RatingView", ratingModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        #endregion
    }
}
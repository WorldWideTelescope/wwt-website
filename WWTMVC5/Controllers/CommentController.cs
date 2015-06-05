//-----------------------------------------------------------------------
// <copyright file="CommentController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the comment partial view request which makes request to repository and gets/publishes the
    /// required data about comment and pushes them to the View.
    /// </summary>
    public class CommentController : ControllerBase
    {
        #region Member variables

        /// <summary>
        /// Instance of Comment Service
        /// </summary>
        private ICommentService _commentService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService _notificationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommentController class.
        /// </summary>
        /// <param name="commentService">Instance of Comment Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        /// <param name="notificationService">Instance of Notification service</param>
        public CommentController(ICommentService commentService, IProfileService profileService, INotificationService notificationService)
            : base(profileService)
        {
            this._commentService = commentService;
            this._notificationService = notificationService;
        }

        #endregion

        #region Action methods

        /// <summary>
        /// Save Action is the action that saves the comment to the database and returns the list of comments to be
        /// shown on UI .
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public bool Save(long entityId, EntityType entityType, string commentText)
        {
            if (!string.IsNullOrWhiteSpace(commentText))
            {
                // Sending data to business logic
                CommentDetails comment = new CommentDetails()
                {
                    Comment = Server.UrlDecode(commentText),
                    ParentID = entityId,
                    CommentedByID = CurrentUserId
                };

                switch (entityType)
                {
                    case EntityType.All:
                        break;
                    case EntityType.Community:
                    case EntityType.Folder:
                        this._commentService.CreateCommunityComment(comment);
                        break;
                    case EntityType.Content:
                        this._commentService.CreateContentComment(comment);
                        break;
                    default:
                        break;
                }

                // TODO: Only on succeeded we need to send the notification email.
                SendEntityCommentMail(comment, entityType);
            }

            return true;
        }

        /// <summary>
        /// Delete Action is the action that soft deletes the comment from database.
        /// </summary>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public void Delete(long entityId, long commentId, EntityType entityType, Permission userPermission, int currentPage)
        {
            try
            {
                switch (entityType)
                {
                    case EntityType.All:
                        break;
                    case EntityType.Community:
                    case EntityType.Folder:
                        this._commentService.DeleteCommunityComment(commentId);
                        break;
                    case EntityType.Content:
                        this._commentService.DeleteContentComment(commentId);
                        break;
                    default:
                        break;
                }

                CommentViewModel commentModel = GetComments(entityId, entityType, userPermission, currentPage, OrderType.NewestFirst);
                PartialView("CommentView", commentModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// It renders the comment partial view
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity Type (Community/Content)</param>
        /// <param name="userPermission">Current Users permission on the comment's community</param>
        [ChildActionOnly]
        public void Render(long entityId, EntityType entityType, Permission userPermission)
        {
            try
            {
                CommentViewModel commentModel = GetComments(entityId, entityType, userPermission, 1, OrderType.NewestFirst);

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("CommentView", commentModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// It renders the comment partial view
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity Type (Community/Content)</param>
        /// <param name="userPermission">Current Users permission on the comment's community</param>
        /// <param name="currentPage">Selected page</param>
        [HttpPost]
        public void AjaxRender(long entityId, EntityType entityType, Permission userPermission, int currentPage)
        {
            try
            {
                CommentViewModel commentModel = GetComments(entityId, entityType, userPermission, currentPage, OrderType.NewestFirst);

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("CommentView", commentModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets comments for the specified entity.
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity Type (Community/Content)</param>
        /// <param name="userPermission">Current Users permission on the comment's community</param>
        /// <param name="currentPage">Selected page</param>
        /// <param name="orderType">Order Type (NewestFirst/OldestFirst)</param>
        /// <returns>List of comments for the specified entity.</returns>
        private CommentViewModel GetComments(long entityId, EntityType entityType, Permission userPermission, int currentPage, OrderType orderType)
        {
            PageDetails pageDetails = this.GetPageDetails(entityId, entityType, orderType, currentPage);
            CommentFilter filter = new CommentFilter(orderType, entityId);

            CommentViewModel commentModel = new CommentViewModel();

            if (pageDetails != null && pageDetails.TotalCount > 0)
            {
                // Gets the total items satisfying the
                IEnumerable<CommentDetails> comments = null;

                switch (entityType)
                {
                    case EntityType.Community:
                    case EntityType.Folder:
                        comments = this._commentService.GetCommunityComments(filter, pageDetails);
                        break;

                    case EntityType.Content:
                        comments = this._commentService.GetContentComments(filter, pageDetails);
                        break;

                    default:
                        break;
                }

                if (comments != null)
                {
                    foreach (var item in comments)
                    {
                        CommentItemViewModel commentItemViewModel = new CommentItemViewModel();
                        Mapper.Map(item, commentItemViewModel);

                        commentItemViewModel.UserImageLink = item.CommentedByPictureID.HasValue ?
                                Url.Action("Thumbnail", "File", new { id = item.CommentedByPictureID }) : "~/Content/Images/profile.png";

                        commentItemViewModel.CanDelete = (userPermission.CanWrite() || CurrentUserId == item.CommentedByID);
                        commentModel.Comments.Add(commentItemViewModel);
                    }
                }
            }

            commentModel.PaginationDetails = pageDetails;
            return commentModel;
        }

        /// <summary>
        /// Gets the page details instance.
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity Type (Community/Content)</param>
        /// <param name="orderType">Order Type (NewestFirst/OldestFirst)</param>
        /// <param name="currentPage">Selected page</param>
        /// <returns>Page details instance</returns>
        private PageDetails GetPageDetails(long entityId, EntityType entityType, OrderType orderType, int currentPage)
        {
            PageDetails pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.CommentsPerPage;

            CommentFilter filter = new CommentFilter(orderType, entityId);
            int totalItemsForCondition = 0;
            switch (entityType)
            {
                case EntityType.Community:
                case EntityType.Folder:
                    totalItemsForCondition = this._commentService.GetTotalCommunityComments(filter);
                    break;
                case EntityType.Content:
                    totalItemsForCondition = this._commentService.GetTotalContentComments(filter);
                    break;
                default:
                    break;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);
            pageDetails.CurrentPage = currentPage > pageDetails.TotalPages ? pageDetails.TotalPages : currentPage;
            pageDetails.TotalCount = totalItemsForCondition;

            return pageDetails;
        }

        /// <summary>
        /// Send Join community mail.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendEntityCommentMail(CommentDetails details, EntityType entityType)
        {
            try
            {
                // Send Mail.
                EntityCommentRequest request = new EntityCommentRequest()
                {
                    ID = details.ParentID,
                    EntityType = entityType,
                    UserComments = details.Comment,
                    UserID = details.CommentedByID,
                    UserName = details.CommentedBy,
                    UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), details.CommentedByID),
                };

                switch (entityType)
                {
                    case EntityType.Community:
                    case EntityType.Folder:
                        request.Link = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), details.ParentID);
                        break;
                    default:
                        request.Link = string.Format(CultureInfo.InvariantCulture, "{0}{1}/Index/{2}", HttpContext.Request.Url.GetServerLink(), entityType.ToString(), details.ParentID);
                        break;
                }

                this._notificationService.NotifyEntityComment(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        #endregion
    }
}
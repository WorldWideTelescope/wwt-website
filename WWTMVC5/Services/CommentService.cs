//-----------------------------------------------------------------------
// <copyright file="CommentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Comment Service having methods for retrieving Comment
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class CommentService : ICommentService
    {
        #region Member Variables

        /// <summary>
        /// Instance of ContentCommentRepository repository
        /// </summary>
        private IContentCommentsRepository contentCommentsRepository;

        /// <summary>
        /// Instance of CommunityCommentRepository repository
        /// </summary>
        private ICommunityCommentRepository communityCommentRepository;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommentService class.
        /// </summary>
        /// <param name="contentCommentsRepository">Instance of ContentComments repository</param>
        public CommentService(IContentCommentsRepository contentCommentsRepository, ICommunityCommentRepository communityCommentRepository)
        {
            this.contentCommentsRepository = contentCommentsRepository;
            this.communityCommentRepository = communityCommentRepository;
        }
      
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all the comments for the specified Content.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all comments of the Content</returns>
        public IEnumerable<CommentDetails> GetContentComments(CommentFilter filter, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { filter, pageDetails });

            Func<ContentComments, object> orderBy = (contentComments) => contentComments.CommentDatetime;
            Expression<Func<ContentComments, bool>> condition = (contentComments) => contentComments.ContentID == filter.EntityId && contentComments.IsDeleted == false;

            IEnumerable<ContentComments> comments = this.contentCommentsRepository
                .GetItems(condition, orderBy, filter.OrderType == OrderType.NewestFirst, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            List<CommentDetails> commentDetails = new List<CommentDetails>();
            if (comments != null)
            {
                foreach (var item in comments)
                {
                    var comment = new CommentDetails();
                    Mapper.Map(item, comment);
                    commentDetails.Add(comment);
                }
            }

            return commentDetails;
        }

        /// <summary>
        /// Gets all the comments for the specified Community.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all comments of the Community</returns>
        public IEnumerable<CommentDetails> GetCommunityComments(CommentFilter filter, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { filter, pageDetails });

            Func<CommunityComments, object> orderBy = (communityComments) => communityComments.CommentedDatetime;
            Expression<Func<CommunityComments, bool>> condition = (communityComments) => communityComments.CommunityID == filter.EntityId && communityComments.IsDeleted == false;

            // Gets the total items satisfying the
            int totalItemsForCondition = this.communityCommentRepository.GetItemsCount(condition);
            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            IEnumerable<CommunityComments> comments = this.communityCommentRepository
                .GetItems(condition, orderBy, filter.OrderType == OrderType.NewestFirst, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            List<CommentDetails> commentDetails = new List<CommentDetails>();
            if (comments != null)
            {
                foreach (var item in comments)
                {
                    var comment = new CommentDetails();
                    Mapper.Map(item, comment);
                    commentDetails.Add(comment);
                }
            }

            return commentDetails;
        }

        /// <summary>
        /// Gets the total number of community comments for the given filter.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <returns>total number of community comments.</returns>
        public int GetTotalCommunityComments(CommentFilter filter)
        {
            this.CheckNotNull(() => new { filter });

            Expression<Func<CommunityComments, bool>> condition = (communityComments) => communityComments.CommunityID == filter.EntityId && communityComments.IsDeleted == false;
            return this.communityCommentRepository.GetItemsCount(condition);
        }

        /// <summary>
        /// Gets the total number of content comments for the given filter.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <returns>total number of content comments.</returns>
        public int GetTotalContentComments(CommentFilter filter)
        {
            this.CheckNotNull(() => new { filter });

            Expression<Func<ContentComments, bool>> condition = (contentComments) => contentComments.ContentID == filter.EntityId && contentComments.IsDeleted == false;
            return this.contentCommentsRepository.GetItemsCount(condition);
        }

        /// <summary>
        /// Creates comments for the community
        /// </summary>
        /// <param name="comment">Details of the comment.</param>
        /// <returns>True if the Comment was created successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool CreateCommunityComment(CommentDetails comment)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { comment });

            try
            {
                CommunityComments communityComment = new CommunityComments();
                Mapper.Map(comment, communityComment);

                communityComment.IsDeleted = false;
                communityComment.CommentedDatetime = DateTime.UtcNow;

                this.communityCommentRepository.Add(communityComment);
                this.communityCommentRepository.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return false;
        }

        /// <summary>
        /// Creates comments for the content
        /// </summary>
        /// <param name="comment">Details of the comment.</param>
        /// <returns>True if the Comment was created successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool CreateContentComment(CommentDetails comment)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { comment });

            try
            {
                ContentComments contentComment = new ContentComments();
                Mapper.Map(comment, contentComment);

                contentComment.IsDeleted = false;
                contentComment.CommentDatetime = DateTime.UtcNow;

                this.contentCommentsRepository.Add(contentComment);
                this.contentCommentsRepository.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return false;
        }

        /// <summary>
        /// Deletes the comment on the community.
        /// </summary>
        /// <param name="communityCommentsID">Id of the comment.</param>
        /// <returns>True if the Comment was deleted successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool DeleteCommunityComment(long communityCommentsID)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { communityCommentsID });

            try
            {
                CommunityComments communityComment = this.communityCommentRepository.GetItem(comment => comment.CommunityCommentsID == communityCommentsID);
                if (communityComment != null)
                {
                    // We are doing only soft delete of comments.
                    communityComment.IsDeleted = true;
                    this.communityCommentRepository.Update(communityComment);

                    this.communityCommentRepository.SaveChanges();
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
        /// Deletes the comment on the content.
        /// </summary>
        /// <param name="contentCommentsID">Id of the comment.</param>
        /// <returns>True if the Comment was deleted successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public bool DeleteContentComment(long contentCommentsID)
        {
            // Make sure input is not null
            this.CheckNotNull(() => new { communityCommentsID = contentCommentsID });

            try
            {
                ContentComments contentComment = this.contentCommentsRepository.GetItem(comment => comment.ContentCommentsID == contentCommentsID);
                if (contentComment != null)
                {
                    // We are doing only soft delete of comments.
                    contentComment.IsDeleted = true;
                    this.contentCommentsRepository.Update(contentComment);

                    this.contentCommentsRepository.SaveChanges();
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
    }
}
//-----------------------------------------------------------------------
// <copyright file="ICommentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the comment service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Gets all the comments for the specified Content.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all comments of the Content</returns>
        IEnumerable<CommentDetails> GetContentComments(CommentFilter filter, PageDetails pageDetails);

        /// <summary>
        /// Gets all the comments for the specified Community.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all comments of the Community</returns>
        IEnumerable<CommentDetails> GetCommunityComments(CommentFilter filter, PageDetails pageDetails);

        /// <summary>
        /// Gets the total number of community comments for the given filter.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <returns>total number of community comments.</returns>
        int GetTotalCommunityComments(CommentFilter filter);

        /// <summary>
        /// Gets the total number of content comments for the given filter.
        /// </summary>
        /// <param name="filter">Filter which comments to be fetched</param>
        /// <returns>total number of content comments.</returns>
        int GetTotalContentComments(CommentFilter filter);

        /// <summary>
        /// Creates comments for the community
        /// </summary>
        /// <param name="comment">Details of the comment.</param>
        /// <returns>True if the Comment was created successfully; Otherwise false.</returns>
        bool CreateCommunityComment(CommentDetails comment);

        /// <summary>
        /// Creates comments for the content
        /// </summary>
        /// <param name="comment">Details of the comment.</param>
        /// <returns>True if the Comment was created successfully; Otherwise false.</returns>
        bool CreateContentComment(CommentDetails comment);

        /// <summary>
        /// Deletes the comment on the community.
        /// </summary>
        /// <param name="communityCommentsID">Id of the comment.</param>
        /// <returns>True if the Comment was deleted successfully; Otherwise false.</returns>
        bool DeleteCommunityComment(long communityCommentsID);

        /// <summary>
        /// Deletes the comment on the content.
        /// </summary>
        /// <param name="contentCommentsID">Id of the comment.</param>
        /// <returns>True if the Comment was deleted successfully; Otherwise false.</returns>
        bool DeleteContentComment(long contentCommentsID);
    }
}
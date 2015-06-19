//-----------------------------------------------------------------------
// <copyright file="IContentRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the content repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IContentRepository : IRepositoryBase<Content>
    {
        /// <summary>
        /// Gets the content specified by the content id. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="contentId">Id of the content.</param>
        /// <returns>Content instance.</returns>
        Content GetContent(long contentId);

        Content GetContent(Guid azureId);

        /// <summary>
        /// Retrieves the multiple instances of contents for the given IDs. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="contentIDs">Content IDs</param>
        /// <returns>Collection of Contents</returns>
        IEnumerable<Content> GetItems(IEnumerable<long> contentIDs);

        /// <summary>
        /// Gets the access type for the given content.
        /// </summary>
        /// <param name="contentId">Content for which access type has to be returned</param>
        /// <returns>Access type of the content</returns>
        string GetContentAccessType(long contentId);

        /// <summary>
        /// Retrieves the latest content IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestContentIDs(int count);
    }
}
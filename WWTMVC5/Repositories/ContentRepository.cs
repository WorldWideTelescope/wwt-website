//-----------------------------------------------------------------------
// <copyright file="ContentRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Content repository having methods for adding/retrieving/deleting/updating
    /// metadata about Content from SQL Azure Layerscape database.
    /// </summary>
    public class ContentRepository : RepositoryBase<Content>, IContentRepository
    {
        /// <summary>
        /// Initializes a new instance of the ContentRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context.
        /// </param>
        public ContentRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets the content specified by the content id. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="contentId">Id of the content.</param>
        /// <returns>Content instance.</returns>
        public Content GetContent(long contentId)
        {
            var content = EarthOnlineDbContext.Content.Where(item => item.ContentID == contentId && item.IsDeleted == false)
                .Include(c => c.AccessType)
                .Include(c => c.ContentRatings)
                .Include(c => c.ContentTags.Select(ct => ct.Tag))
                .Include(c => c.User)
                .Include(cr => cr.ContentRelation.Select(r => r.Content1))
                .Include(cc => cc.CommunityContents.Select(r => r.Community)).FirstOrDefault();

            return content;
        }

        /// <summary>
        /// Gets the content specified by the content id. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="azureId">azure guid of the content.</param>
        /// <returns>Content instance.</returns>
        public Content GetContent(Guid azureId)
        {
            var content = EarthOnlineDbContext.Content.Where(item => item.ContentAzureID == azureId && item.IsDeleted == false)
                .Include(c => c.AccessType)
                .Include(c => c.ContentRatings)
                .Include(c => c.ContentTags.Select(ct => ct.Tag))
                .Include(c => c.User)
                .Include(cr => cr.ContentRelation.Select(r => r.Content1))
                .Include(cc => cc.CommunityContents.Select(r => r.Community)).FirstOrDefault();

            return content;
        }

        /// <summary>
        /// Retrieves the multiple instances of contents for the given IDs. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="contentIDs">Content IDs</param>
        /// <returns>Collection of Contents</returns>
        public IEnumerable<Content> GetItems(IEnumerable<long> contentIDs)
        {
            IEnumerable<Content> result = DbSet.Where(content => contentIDs.Contains(content.ContentID))
                .Include(cc => cc.ContentRatings)
                .Include(cr => cr.ContentRelation.Select(r => r.Content1))
                .Include(cc => cc.CommunityContents)
                .Include(cc => cc.ContentTags.Select(t => t.Tag)).OrderByDescending(content => content.ModifiedDatetime);

            return result.ToList();
        }

        /// <summary>
        /// Gets the access type for the given content.
        /// </summary>
        /// <param name="contentId">Content for which access type has to be returned</param>
        /// <returns>Access type of the content</returns>
        public string GetContentAccessType(long contentId)
        {
            var query = "SELECT dbo.GetContentAccessType(@contentID)";
            return EarthOnlineDbContext.Database.SqlQuery<string>(query, new SqlParameter("contentID", contentId)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the latest content IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        public IEnumerable<long> GetLatestContentIDs(int count)
        {
            // Get the contents which are not deleted.
            var result = EarthOnlineDbContext.Content.Where(content => !(bool)content.IsDeleted &&
                                                                            content.AccessTypeID == (int)AccessType.Public).OrderByDescending(content => content.ContentID).Select(content => content.ContentID)
                                .Take(count);

            return result.ToList();
        }
    }
}
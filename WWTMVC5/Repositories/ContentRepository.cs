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
        /// <param name="contentID">Id of the content.</param>
        /// <returns>Content instance.</returns>
        public Content GetContent(long contentID)
        {
            var content = Queryable.Where<Content>(this.EarthOnlineDbContext.Content, item => item.ContentID == contentID && item.IsDeleted == false)
                .Include(c => c.AccessType)
                .Include<Content, ICollection<ContentRatings>>(c => c.ContentRatings)
                .Include(c => Enumerable.Select<ContentTags, Tag>(c.ContentTags, ct => ct.Tag))
                .Include<Content, User>(c => c.User)
                .Include(cr => Enumerable.Select<ContentRelation, Content>(cr.ContentRelation, r => r.Content1))
                .Include(cc => Enumerable.Select<CommunityContents, Community>(cc.CommunityContents, r => r.Community)).FirstOrDefault();

            return content;
        }

        /// <summary>
        /// Retrieves the multiple instances of contents for the given IDs. Eager loads the navigation properties to avoid multiple calls to DB.
        /// </summary>
        /// <param name="contentIDs">Content IDs</param>
        /// <returns>Collection of Contents</returns>
        public IEnumerable<Content> GetItems(IEnumerable<long> contentIDs)
        {
            IEnumerable<Content> result = null;

            result = Queryable.OrderByDescending<Content, DateTime?>(Queryable.Where(this.DbSet, content => contentIDs.Contains(content.ContentID))
                                    .Include<Content, ICollection<ContentRatings>>(cc => cc.ContentRatings)
                                    .Include(cr => Enumerable.Select<ContentRelation, Content>(cr.ContentRelation, r => r.Content1))
                                    .Include<Content, ICollection<CommunityContents>>(cc => cc.CommunityContents)
                                    .Include(cc => Enumerable.Select<ContentTags, Tag>(cc.ContentTags, t => t.Tag)), content => content.ModifiedDatetime);

            return result.ToList();
        }

        /// <summary>
        /// Gets the access type for the given content.
        /// </summary>
        /// <param name="contentID">Content for which access type has to be returned</param>
        /// <returns>Access type of the content</returns>
        public string GetContentAccessType(long contentID)
        {
            string query = "SELECT dbo.GetContentAccessType(@contentID)";
            return this.EarthOnlineDbContext.Database.SqlQuery<string>(query, new SqlParameter("contentID", contentID)).FirstOrDefault();
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
            var result = Queryable.Select<Content, long>(Queryable.OrderByDescending<Content, long>(Queryable.Where<Content>(this.EarthOnlineDbContext.Content, content => !(bool)content.IsDeleted &&
                                                                                                                                                                         content.AccessTypeID == (int)AccessType.Public), content => content.ContentID), content => content.ContentID)
                                .Take(count);

            return result.ToList();
        }
    }
}
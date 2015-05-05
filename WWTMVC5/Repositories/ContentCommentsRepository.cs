//-----------------------------------------------------------------------
// <copyright file="ContentCommentsRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Content Comments repository having methods for retrieving comments of contents
    /// from SQL Azure Layerscape database.
    /// </summary>
    public class ContentCommentsRepository : RepositoryBase<ContentComments>, IContentCommentsRepository
    {
        /// <summary>
        /// Initializes a new instance of the ContentCommentsRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Instance of Layerscape db context</param>
        public ContentCommentsRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets list of all users who have commented on the given Content.
        /// </summary>
        /// <param name="contentID">Content ID.</param>
        /// <returns>List of all user who have commented on the Content.</returns>
        public IEnumerable<User> GetCommenters(long contentID)
        {
            var commenters = Queryable.Distinct<User>((from comments in this.EarthOnlineDbContext.ContentComments
                                  where comments.ContentID == contentID && comments.IsDeleted == false
                                  select comments.User));

            return commenters.ToList();
        }
    }
}
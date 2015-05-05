//-----------------------------------------------------------------------
// <copyright file="CommunityCommentRepository.cs" company="Microsoft Corporation">
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
    /// Class representing the Community Comment repository having methods for retrieving comments for communities
    /// from SQL Azure Layerscape database.
    /// </summary>
    public class CommunityCommentRepository : RepositoryBase<CommunityComments>, ICommunityCommentRepository
    {
        /// <summary>
        /// Initializes a new instance of the CommunityCommentRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context
        /// </param>
        public CommunityCommentRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Gets list of all users who have commented on the given community.
        /// </summary>
        /// <param name="communityID">Community ID.</param>
        /// <returns>List of all user who have commented on the community.</returns>
        public IEnumerable<User> GetCommenters(long communityID)
        {
            var commenters = Queryable.Distinct<User>((from comments in this.EarthOnlineDbContext.CommunityComments
                                  where comments.CommunityID == communityID && comments.IsDeleted == false
                                  select comments.User));

            return commenters.ToList();
        }
    }
}
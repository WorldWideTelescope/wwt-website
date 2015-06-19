//-----------------------------------------------------------------------
// <copyright file="ICommunityCommentRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the community comment repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommunityCommentRepository : IRepositoryBase<CommunityComments>
    {
        /// <summary>
        /// Gets list of all users who have commented on the given community.
        /// </summary>
        /// <param name="communityId">Community ID.</param>
        /// <returns>List of all user who have commented on the community.</returns>
        IEnumerable<User> GetCommenters(long communityId);
    }
}
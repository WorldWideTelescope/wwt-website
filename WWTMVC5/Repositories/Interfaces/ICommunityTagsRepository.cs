//-----------------------------------------------------------------------
// <copyright file="ICommunityTagsRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the CommunityTags repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommunityTagsRepository : IRepositoryBase<CommunityTags>
    {
        /// <summary>
        /// Gets the Ids of the related communities of the given community. Related communities are taken based on the
        /// tags which are matching between the tags of given community
        /// </summary>
        /// <param name="communityId">Id of the Community.</param>
        /// <param name="userId">User who is requesting the related communities</param>
        /// <returns>Ids of related communities.</returns>
        IEnumerable<long> GetRelatedCommunityIDs(long communityId, long userId);
    }
}
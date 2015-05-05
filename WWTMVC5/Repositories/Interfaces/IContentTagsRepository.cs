//-----------------------------------------------------------------------
// <copyright file="IContentTagsRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the ContentTags repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IContentTagsRepository : IRepositoryBase<ContentTags>
    {
        /// <summary>
        /// Gets the Ids of the related content of the given content. Related contents are taken based on the
        /// tags which are matching between the tags of given content.
        /// </summary>
        /// <param name="contentID">Id of the Content.</param>
        /// <param name="userID">User who is requesting the related contents</param>
        /// <returns>Ids of related contents.</returns>
        IEnumerable<long> GetRelatedContentIDs(long contentID, long userID);
    }
}
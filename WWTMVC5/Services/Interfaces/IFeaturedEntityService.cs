//-----------------------------------------------------------------------
// <copyright file="IFeaturedEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the entity service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IFeaturedEntityService
    {
        /// <summary>
        /// This function is used to update the featured communities.
        /// </summary>
        /// <param name="communities">List of all communities.</param>
        /// <param name="userId">ID of the user who is updating the communities.</param>
        /// <param name="categoryId">Category Type.</param>
        /// <returns>True of the communities are updated. False otherwise.</returns>
        Task<OperationStatus> UpdateFeaturedCommunities(IEnumerable<AdminEntityDetails> communities, long userId, int? categoryId);

        /// <summary>
        /// This function is used to update the featured contents.
        /// </summary>
        /// <param name="contents">List of all contents.</param>
        /// <param name="userId">ID of the user who is updating the contents.</param>
        /// <param name="categoryId">Category Type.</param>
        /// <returns>True of the contents are updated. False otherwise.</returns>
        Task<OperationStatus> UpdateFeaturedContents(IEnumerable<AdminEntityDetails> contents, long userId, int? categoryId);
    }
}
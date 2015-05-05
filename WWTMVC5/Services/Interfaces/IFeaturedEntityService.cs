//-----------------------------------------------------------------------
// <copyright file="IFeaturedEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
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
        /// <param name="userID">ID of the user who is updating the communities.</param>
        /// <param name="categoryID">Category Type.</param>
        /// <returns>True of the communities are updated. False otherwise.</returns>
        OperationStatus UpdateFeaturedCommunities(IEnumerable<AdminEntityDetails> communities, long userID, int? categoryID);

        /// <summary>
        /// This function is used to update the featured contents.
        /// </summary>
        /// <param name="contents">List of all contents.</param>
        /// <param name="userID">ID of the user who is updating the contents.</param>
        /// <param name="categoryID">Category Type.</param>
        /// <returns>True of the contents are updated. False otherwise.</returns>
        OperationStatus UpdateFeaturedContents(IEnumerable<AdminEntityDetails> contents, long userID, int? categoryID);
    }
}
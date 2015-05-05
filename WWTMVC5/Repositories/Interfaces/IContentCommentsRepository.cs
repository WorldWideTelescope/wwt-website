//-----------------------------------------------------------------------
// <copyright file="IContentCommentsRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the content comment repository methods. Also, needed for adding unit test cases.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "need to have this class for testing purpose.")]
    public interface IContentCommentsRepository : IRepositoryBase<ContentComments>
    {
        /// <summary>
        /// Gets list of all users who have commented on the given Content.
        /// </summary>
        /// <param name="contentID">Content ID.</param>
        /// <returns>List of all user who have commented on the Content.</returns>
        IEnumerable<User> GetCommenters(long contentID);
    }
}
//-----------------------------------------------------------------------
// <copyright file="IReportEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the report entity service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IReportEntityService
    {
        /// <summary>
        /// Reports a community as offensive.
        /// </summary>
        /// <param name="offensiveCommunityDetails">Details about the community and its offensive report.</param>
        /// <returns>True if community was reported as offensive; otherwise false.</returns>
        OperationStatus ReportOffensiveCommunity(ReportEntityDetails offensiveCommunityDetails);

        /// <summary>
        /// Reports a content as offensive.
        /// </summary>
        /// <param name="offensiveContentDetails">Details about the content and its offensive report.</param>
        /// <returns>True if content was reported as offensive; otherwise false.</returns>
        OperationStatus ReportOffensiveContent(ReportEntityDetails offensiveContentDetails);

        /// <summary>
        /// Get all Offensive communities.
        /// </summary>
        /// <param name="userID">Id of the user</param>
        /// <returns>List of all offensive communities.</returns>
        IEnumerable<OffensiveEntityDetails> GetOffensiveCommunities(long userID);

        /// <summary>
        /// Get all Offensive contents.
        /// </summary>
        /// <param name="userID">Id of the user</param>
        /// <returns>List of all offensive contents.</returns>
        IEnumerable<OffensiveEntityDetails> GetOffensiveContents(long userID);

        /// <summary>
        /// Updates the Community Entry with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if Community was updated; otherwise false.</returns>
        OperationStatus UpdateOffensiveCommunityEntry(OffensiveEntry details);

        /// <summary>
        /// Updates the content Entry with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        OperationStatus UpdateOffensiveContentEntry(OffensiveEntry details);

        /// <summary>
        /// Updates the all the entries for the given Community with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if Community was updated; otherwise false.</returns>
        OperationStatus UpdateAllOffensiveCommunityEntry(OffensiveEntry details);

        /// <summary>
        /// Updates the all the entries for the given Content with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        OperationStatus UpdateAllOffensiveContentEntry(OffensiveEntry details);
    }
}
//-----------------------------------------------------------------------
// <copyright file="IStaticContentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the rating service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IStaticContentService
    {
        /// <summary>
        /// Update Static Content
        /// </summary>
        /// <param name="staticContentDetails">static content details.</param>
        /// <returns>True if the content was updated successfully; Otherwise false.</returns>
        OperationStatus UpdateStaticContent(StaticContentDetails staticContentDetails);

        /// <summary>
        /// Gets static content from the DB
        /// </summary>
        /// <param name="staticContentType">static Content Type</param>
        /// <returns>static content object</returns>
        StaticContentDetails GetStaticContent(StaticContentType staticContentType);
    }
}
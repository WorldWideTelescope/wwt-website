//-----------------------------------------------------------------------
// <copyright file="IContentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the content service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// Gets the contents from the Layerscape database.
        /// </summary>
        /// <param name="contentId">Content for which details to be fetched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Details about the content</returns>
        ContentDetails GetContentDetails(long contentId, long userId);

        /// <summary>
        /// Gets the contents from the Layerscape database.
        /// </summary>
        /// <param name="contentId">Content for which details to be fetched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Details about the content</returns>
        ContentDetails GetContentDetailsForEdit(long contentId, long userId);

        /// <summary>
        /// Creates the new content in Layerscape with the given details passed in contentDetails instance.
        /// </summary>
        /// <param name="contentDetails">Details of the content</param>
        /// <returns>Id of the content created. Returns -1 is creation is failed.</returns>
        long CreateContent(ContentDetails contentDetails);

        /// <summary>
        /// Updates the content in Layerscape with the given details passed in contentDetails instance.
        /// </summary>
        /// <param name="contentDetails">Details of the content</param>
        /// <param name="userId">User identification</param>
        /// <returns>True if content is updated; otherwise false.</returns>
        bool UpdateContent(ContentDetails contentDetails, long userId);

        /// <summary>
        /// Deletes the specified content from the Earth database.
        /// </summary>
        /// <param name="contentId">Content Id</param>
        /// <param name="profileId">User Identity</param>
        /// <param name="isOffensive">Whether content is offensive or not.</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus DeleteContent(long contentId, long profileId, bool isOffensive, OffensiveEntry offensiveDetails);

        /// <summary>
        /// Deletes the specified content from the Earth database.
        /// </summary>
        /// <param name="contentId">Content Id</param>
        /// <param name="profileId">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus DeleteContent(long contentId, long profileId);

        /// <summary>
        /// Un-deletes the specified content from the Earth database so that it is again accessible in the site.
        /// </summary>
        /// <param name="contentId">Content Id</param>
        /// <param name="userId">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus UnDeleteOffensiveContent(long contentId, long userId);

        /// <summary>
        /// Sets the given access type for the specified Content.
        /// </summary>
        /// <param name="contentId">Content Id</param>
        /// <param name="userId">User Identity</param>
        /// <param name="accessType">Access type of the Content.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        OperationStatus SetContentAccessType(long contentId, long userId, AccessType accessType);

        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        bool UploadTemporaryFile(FileDetail fileDetail);

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="userId">User for which the parent communities/folders are being fetched</param>
        /// <returns>List of communities folders</returns>
        IEnumerable<Community> GetParentCommunities(long userId);

        /// <summary>
        /// Increments the download count of the content identified by the ContentId.
        /// </summary>
        /// <param name="contentId">Content ID</param>
        /// <param name="userId">Current user id</param>
        void IncrementDownloadCount(long contentId, long userId);

        /// <summary>
        /// This function retrieves the contents uploaded by the user.
        /// </summary>
        /// <param name="userId">User identity.</param>
        /// <returns>Payload details.</returns>
        Task<PayloadDetails> GetUserContents(long userId);

        /// <summary>
        /// Gets the role of the user on the given Content.
        /// </summary>
        /// <param name="content">Content on which user role has to be found</param>
        /// <param name="userId">Current user id</param>
        /// <returns>UserRole on the content</returns>
        UserRole GetContentUserRole(Content content, long? userId);

        /// <summary>
        /// Retrieves the latest content IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        IEnumerable<long> GetLatestContentIDs(int count);
    }
}

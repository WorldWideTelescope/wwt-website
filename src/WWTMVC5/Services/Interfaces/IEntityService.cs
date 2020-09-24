//-----------------------------------------------------------------------
// <copyright file="IEntityService.cs" company="Microsoft Corporation">
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
    public interface IEntityService
    {
        /// <summary>
        /// Gets the communities from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all communities</returns>
        Task<IEnumerable<CommunityDetails>> GetCommunities(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails);

        /// <summary>
        /// Gets all the communities from the Layerscape database including the deleted items.
        /// </summary>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="categoryId">Category ID for which communities to be fetched</param>
        /// <returns>List of all communities</returns>
        Task<IEnumerable<CommunityDetails>> GetAllCommunities(long userId, int? categoryId);

        /// <summary>
        /// Gets the content from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all contents</returns>
        Task<IEnumerable<ContentDetails>> GetContents(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails);

        /// <summary>
        /// Gets all the contents from the Layerscape database including the deleted items.
        /// </summary>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="categoryId">Category ID for which contents to be fetched</param>
        /// <returns>List of all contents</returns>
        Task<IEnumerable<ContentDetails>> GetAllContents(long userId, int? categoryId);

        /// <summary>
        /// Gets the communities from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>List of all communities</returns>
        Task<IEnumerable<CommunityDetails>> GetCommunities(EntityHighlightFilter entityHighlightFilter);

        /// <summary>
        /// Gets the content from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>List of all contents</returns>
        Task<IEnumerable<ContentDetails>> GetContents(EntityHighlightFilter entityHighlightFilter);

        /// <summary>
        /// Gets the top categories from the Layerscape database based on the number of contents which belongs to the category.
        /// </summary>
        /// <returns>List of top 6 categories</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "We need to apply filters so we need to have it as a method.")]
        Task<IEnumerable<EntityDetails>> GetTopCategories();

        /// <summary>
        /// Retrieves the sub communities of a given community. This only retrieves the immediate children.
        /// </summary>
        /// <param name="communityId">ID of the community.</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyItemCount">To get only item count, not the entities. When community details page is loaded first time, 
        /// no need to get the communities, only count is enough.</param>
        /// <returns>Collection of sub communities</returns>
        IEnumerable<CommunityDetails> GetSubCommunities(long communityId, long userId, PageDetails pageDetails, bool onlyItemCount);

        /// <summary>
        /// Retrieves the contents of the given community.
        /// </summary>
        /// <param name="communityId">ID of the community.</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>An enumerable which contains the contents of the community</returns>
        IEnumerable<ContentDetails> GetContents(long communityId, long userId, PageDetails pageDetails);
        
        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        bool UploadTemporaryFile(FileDetail fileDetail);
    }
}
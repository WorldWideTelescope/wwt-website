//-----------------------------------------------------------------------
// <copyright file="ContentTagsRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the ContentTags repository having methods for retrieving the ContentTags
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class ContentTagsRepository : RepositoryBase<ContentTags>, IContentTagsRepository
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContentTagsRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context.
        /// </param>
        public ContentTagsRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Gets the Ids of the related content of the given content. Related contents are taken based on the
        /// tags which are matching between the tags of given content.
        /// </summary>
        /// <param name="contentId">Id of the Content.</param>
        /// <param name="userId">User who is requesting the related contents</param>
        /// <returns>Ids of related contents.</returns>
        public IEnumerable<long> GetRelatedContentIDs(long contentId, long userId)
        {
            var userIDstring = string.Format(CultureInfo.InvariantCulture, "~{0}~", Convert.ToString(userId, CultureInfo.InvariantCulture));

            // Considering the performance of the query, Related Contents are fetched using the Search View
            // which will have information about the users who are having access to the content also.
            var query = @"SELECT ContentID FROM ContentTags INNER JOIN SearchView ON ContentID = ID
                                WHERE 
                                        Entity = 'Content'
                                    AND 
                                        TagID IN (SELECT TagID FROM ContentTags WHERE ContentID = @contentID) AND ContentID != @contentID
                                    AND
                                        (AccessType = 'Public' OR Users Like @userID)
                                GROUP BY ContentID, Rating
                                ORDER BY COUNT(ContentID) DESC, Rating DESC";
            return EarthOnlineDbContext.Database.SqlQuery<long>(query, new SqlParameter("contentID", contentId), new SqlParameter("userID", userIDstring)).ToList();
        }

        #endregion Public methods
    }
}
//-----------------------------------------------------------------------
// <copyright file="CommunityTagsRepository.cs" company="Microsoft Corporation">
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
    /// Class representing the CommunityTags repository having methods for retrieving the CommunityTags
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class CommunityTagsRepository : RepositoryBase<CommunityTags>, ICommunityTagsRepository
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommunityTagsRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context.
        /// </param>
        public CommunityTagsRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Gets the Ids of the related communities of the given community. Related communities are taken based on the
        /// tags which are matching between the tags of given community
        /// </summary>
        /// <param name="communityId">Id of the Community.</param>
        /// <param name="userId">User who is requesting the related communities</param>
        /// <returns>Ids of related communities.</returns>
        public IEnumerable<long> GetRelatedCommunityIDs(long communityId, long userId)
        {
            var userIDstring = string.Format(CultureInfo.InvariantCulture, "~{0}~", Convert.ToString(userId, CultureInfo.InvariantCulture));

            // Considering the performance of the query, Related Communities are fetched using the Search View
            // which will have information about the users who are having access to the community also.
            var query = @"SELECT CommunityID FROM CommunityTags INNER JOIN SearchView ON CommunityID = ID
                                WHERE 
                                        Entity = 'Community'
                                    AND 
                                        TagID IN (SELECT TagID FROM CommunityTags WHERE CommunityID = @communityID) AND CommunityId != @communityID
                                    AND
                                        (AccessType = 'Public' OR Users Like @userID)
                                GROUP BY CommunityID, Rating
                                ORDER BY COUNT(CommunityID) DESC, Rating DESC";
            return EarthOnlineDbContext.Database.SqlQuery<long>(query, new SqlParameter("communityID", communityId), new SqlParameter("userID", userIDstring)).ToList();
        }

        #endregion Public methods
    }
}
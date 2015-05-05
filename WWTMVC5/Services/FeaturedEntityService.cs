//-----------------------------------------------------------------------
// <copyright file="FeaturedEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Entity Service having methods for retrieving Entity
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class FeaturedEntityService : IFeaturedEntityService
    {
        #region Members
        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository userRepository;

        /// <summary>
        /// Instance of FeaturedCommunities repository
        /// </summary>
        private IRepositoryBase<FeaturedCommunities> featuredCommunitiesRepository;

        /// <summary>
        /// Instance of FeaturedCommunities repository
        /// </summary>
        private IRepositoryBase<FeaturedContents> featuredContentsRepository;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FeaturedEntityService class.
        /// </summary>
        public FeaturedEntityService(
                IUserRepository userRepository,
                IRepositoryBase<FeaturedCommunities> featuredCommunitiesRepository,
                IRepositoryBase<FeaturedContents> featuredContentsRepository)
        {
            this.userRepository = userRepository;
            this.featuredCommunitiesRepository = featuredCommunitiesRepository;
            this.featuredContentsRepository = featuredContentsRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This function is used to update the featured communities.
        /// </summary>
        /// <param name="communities">List of all communities.</param>
        /// <param name="userID">ID of the user who is updating the communities.</param>
        /// <param name="categoryID">Category Type.</param>
        /// <returns>True of the communities are updated. False otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public OperationStatus UpdateFeaturedCommunities(IEnumerable<AdminEntityDetails> communities, long userID, int? categoryID)
        {
            OperationStatus status = null;
            try
            {
                // TODO: To find a better way of updating and deleting the featured communities.
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    IEnumerable<FeaturedCommunities> results;
                    if (categoryID.HasValue)
                    {
                        results = this.featuredCommunitiesRepository.GetItems(fc => fc.CategoryID == categoryID, null, false);
                    }
                    else
                    {
                        results = this.featuredCommunitiesRepository.GetItems(fc => int.Equals(fc.CategoryID, categoryID), null, false);
                    }

                    if (results != null && results.Count() > 0)
                    {
                        foreach (var item in results)
                        {
                            this.featuredCommunitiesRepository.Delete(item);
                        }
                    }

                    if (communities != null && communities.Count() > 0)
                    {
                        foreach (var item in communities)
                        {
                            FeaturedCommunities community = new FeaturedCommunities()
                            {
                                CategoryID = categoryID,
                                CommunityID = item.EntityID,
                                SortOrder = item.SortOrder,
                                UpdatedByID = userID,
                                UpdatedDatetime = DateTime.UtcNow
                            };

                            this.featuredCommunitiesRepository.Add(community);
                        }
                    }

                    this.featuredCommunitiesRepository.SaveChanges();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// This function is used to update the featured contents.
        /// </summary>
        /// <param name="contents">List of all contents.</param>
        /// <param name="userID">ID of the user who is updating the contents.</param>
        /// <param name="categoryID">Category Type.</param>
        /// <returns>True of the contents are updated. False otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public OperationStatus UpdateFeaturedContents(IEnumerable<AdminEntityDetails> contents, long userID, int? categoryID)
        {
            OperationStatus status = null;
            try
            {
                // TODO: To find a better way of updating and deleting the featured contents.
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    IEnumerable<FeaturedContents> results;
                    if (categoryID.HasValue)
                    {
                        results = this.featuredContentsRepository.GetItems(fc => fc.CategoryID == categoryID, null, false);
                    }
                    else
                    {
                        results = this.featuredContentsRepository.GetItems(fc => int.Equals(fc.CategoryID, categoryID), null, false);
                    }

                    if (results != null && results.Count() > 0)
                    {
                        foreach (var item in results)
                        {
                            this.featuredContentsRepository.Delete(item);
                        }
                    }

                    if (contents != null && contents.Count() > 0)
                    {
                        foreach (var item in contents)
                        {
                            FeaturedContents content = new FeaturedContents()
                            {
                                CategoryID = categoryID,
                                ContentID = item.EntityID,
                                SortOrder = item.SortOrder,
                                UpdatedByID = userID,
                                UpdatedDatetime = DateTime.UtcNow
                            };

                            this.featuredContentsRepository.Add(content);
                        }
                    }

                    this.featuredContentsRepository.SaveChanges();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}

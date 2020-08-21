//-----------------------------------------------------------------------
// <copyright file="FeaturedEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private IUserRepository _userRepository;

        /// <summary>
        /// Instance of FeaturedCommunities repository
        /// </summary>
        private IRepositoryBase<FeaturedCommunities> _featuredCommunitiesRepository;

        /// <summary>
        /// Instance of FeaturedCommunities repository
        /// </summary>
        private IRepositoryBase<FeaturedContents> _featuredContentsRepository;

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
            this._userRepository = userRepository;
            this._featuredCommunitiesRepository = featuredCommunitiesRepository;
            this._featuredContentsRepository = featuredContentsRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This function is used to update the featured communities.
        /// </summary>
        /// <param name="communities">List of all communities.</param>
        /// <param name="userId">ID of the user who is updating the communities.</param>
        /// <param name="categoryId">Category Type.</param>
        /// <returns>True of the communities are updated. False otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public Task<OperationStatus> UpdateFeaturedCommunities(IEnumerable<AdminEntityDetails> communities, long userId, int? categoryId)
        {
            OperationStatus status = null;
            try
            {
                // TODO: To find a better way of updating and deleting the featured communities.
                if (_userRepository.IsSiteAdmin(userId))
                {
                    IEnumerable<FeaturedCommunities> results;
                    if (categoryId.HasValue)
                    {
                        results = _featuredCommunitiesRepository.GetItems(fc => fc.CategoryID == categoryId, null, false);
                    }
                    else
                    {
                        results = _featuredCommunitiesRepository.GetItems(fc => int.Equals(fc.CategoryID, categoryId), null, false);
                    }

                    if (results != null && results.Any())
                    {
                        foreach (var item in results)
                        {
                            _featuredCommunitiesRepository.Delete(item);
                        }
                    }

                    if (communities != null && communities.Any())
                    {
                        foreach (var item in communities)
                        {
                            var community = new FeaturedCommunities()
                            {
                                CategoryID = categoryId,
                                CommunityID = item.EntityID,
                                SortOrder = item.SortOrder,
                                UpdatedByID = userId,
                                UpdatedDatetime = DateTime.UtcNow
                            };

                            _featuredCommunitiesRepository.Add(community);
                        }
                    }

                    _featuredCommunitiesRepository.SaveChanges();
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

            return Task.FromResult(status);
        }

        /// <summary>
        /// This function is used to update the featured contents.
        /// </summary>
        /// <param name="contents">List of all contents.</param>
        /// <param name="userId">ID of the user who is updating the contents.</param>
        /// <param name="categoryId">Category Type.</param>
        /// <returns>True of the contents are updated. False otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public Task<OperationStatus> UpdateFeaturedContents(IEnumerable<AdminEntityDetails> contents, long userId, int? categoryId)
        {
            OperationStatus status = null;
            try
            {
                // TODO: To find a better way of updating and deleting the featured contents.
                if (_userRepository.IsSiteAdmin(userId))
                {
                    IEnumerable<FeaturedContents> results;
                    if (categoryId.HasValue)
                    {
                        results =  _featuredContentsRepository.GetItems(fc => fc.CategoryID == categoryId, null, false);
                    }
                    else
                    {
                        results =  _featuredContentsRepository.GetItems(fc => int.Equals(fc.CategoryID, categoryId), null, false);
                    }

                    if (results != null && results.Any())
                    {
                        foreach (var item in results)
                        {
                            _featuredContentsRepository.Delete(item);
                        }
                    }

                    if (contents != null && contents.Any())
                    {
                        foreach (var item in contents)
                        {
                            var content = new FeaturedContents()
                            {
                                CategoryID = categoryId,
                                ContentID = item.EntityID,
                                SortOrder = item.SortOrder,
                                UpdatedByID = userId,
                                UpdatedDatetime = DateTime.UtcNow
                            };

                            _featuredContentsRepository.Add(content);
                        }
                    }

                    _featuredContentsRepository.SaveChanges();
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

            return Task.FromResult(status);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}

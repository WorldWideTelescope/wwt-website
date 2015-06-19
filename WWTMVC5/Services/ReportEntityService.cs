//-----------------------------------------------------------------------
// <copyright file="ReportEntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the ReportEntity Service having methods for reporting an entity as abusive.
    /// </summary>
    public class ReportEntityService : IReportEntityService
    {
        #region Member Variables

        /// <summary>
        /// Instance of OffensiveContent repository
        /// </summary>
        private IRepositoryBase<OffensiveContent> _offensiveContentRepository;

        /// <summary>
        /// Instance of OffensiveCommunities repository
        /// </summary>
        private IRepositoryBase<OffensiveCommunities> _offensiveCommunitiesRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository _userRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ReportEntityService class.
        /// </summary>
        /// <param name="offensiveContentRepository">Instance of offensiveContent repository</param>
        /// <param name="offensiveCommunitiesRepository">Instance of offensiveCommunities repository</param>
        public ReportEntityService(
            IRepositoryBase<OffensiveContent> offensiveContentRepository,
            IRepositoryBase<OffensiveCommunities> offensiveCommunitiesRepository,
            IUserRepository userRepository)
        {
            this._offensiveCommunitiesRepository = offensiveCommunitiesRepository;
            this._offensiveContentRepository = offensiveContentRepository;
            this._userRepository = userRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reports a community as offensive.
        /// </summary>
        /// <param name="offensiveCommunityDetails">Details about the community and its offensive report.</param>
        /// <returns>True if community was reported as offensive; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus ReportOffensiveCommunity(ReportEntityDetails offensiveCommunityDetails)
        {
            OperationStatus status = null;

            this.CheckNotNull(() => new { reportEntityDetails = offensiveCommunityDetails });
            try
            {
                var offensiveCommunity = new OffensiveCommunities();
                Mapper.Map(offensiveCommunityDetails, offensiveCommunity);

                offensiveCommunity.ReportedDatetime = DateTime.UtcNow;

                _offensiveCommunitiesRepository.Add(offensiveCommunity);
                _offensiveCommunitiesRepository.SaveChanges();
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Reports a content as offensive.
        /// </summary>
        /// <param name="offensiveContentDetails">Details about the content and its offensive report.</param>
        /// <returns>True if content was reported as offensive; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO: Error handling")]
        public OperationStatus ReportOffensiveContent(ReportEntityDetails offensiveContentDetails)
        {
            OperationStatus status = null;

            this.CheckNotNull(() => new { reportEntityDetails = offensiveContentDetails });

            try
            {
                var offensiveContent = new OffensiveContent();
                Mapper.Map(offensiveContentDetails, offensiveContent);

                offensiveContent.ReportedDatetime = DateTime.UtcNow;

                _offensiveContentRepository.Add(offensiveContent);
                _offensiveContentRepository.SaveChanges();
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Get all Offensive communities.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of all offensive communities.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        public async Task<IEnumerable<OffensiveEntityDetails>> GetOffensiveCommunities(long userId)
        {
            var results = new List<OffensiveEntityDetails>();

            try
            {
                if (_userRepository.IsSiteAdmin(userId))
                {
                    var offensiveCommunities =  _offensiveCommunitiesRepository.GetItems(
                        oc => oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged,
                        oc => oc.ReportedDatetime,
                        true);

                    foreach (var item in offensiveCommunities)
                    {
                        var detail = new OffensiveEntityDetails();
                        Mapper.Map(item, detail);
                        results.Add(detail);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return results;
        }

        /// <summary>
        /// Get all Offensive contents.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of all offensive contents.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        public async Task<IEnumerable<OffensiveEntityDetails>> GetOffensiveContents(long userId)
        {
            var results = new List<OffensiveEntityDetails>();

            try
            {
                if (_userRepository.IsSiteAdmin(userId))
                {
                    var offensiveContents =  _offensiveContentRepository.GetItems(
                        oc => oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged,
                        oc => oc.ReportedDatetime,
                        true);

                    foreach (var item in offensiveContents)
                    {
                        var detail = new OffensiveEntityDetails();
                        Mapper.Map(item, detail);
                        results.Add(detail);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Add exception handling logic here.
            }

            return results;
        }

        /// <summary>
        /// Updates the Community Entry with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if Community was updated; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public OperationStatus UpdateOffensiveCommunityEntry(OffensiveEntry details)
        {
            this.CheckNotNull(() => new { details = details });

            OperationStatus status = null;
            try
            {
                if (_userRepository.IsSiteAdmin(details.ReviewerID))
                {
                    var offensiveCommunities = _offensiveCommunitiesRepository.GetItem(oc => oc.OffensiveCommunitiesID == details.EntryID);
                    if (offensiveCommunities != null)
                    {
                        offensiveCommunities.OffensiveStatusID = (int)details.Status;
                        offensiveCommunities.Justification = details.Justification;

                        offensiveCommunities.ReviewerID = details.ReviewerID;
                        offensiveCommunities.ReviewerDatetime = DateTime.UtcNow;

                        _offensiveCommunitiesRepository.Update(offensiveCommunities);
                        _offensiveCommunitiesRepository.SaveChanges();
                    }
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

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Updates the content Entry with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public OperationStatus UpdateOffensiveContentEntry(OffensiveEntry details)
        {
            this.CheckNotNull(() => new { details = details });

            OperationStatus status = null;
            try
            {
                if (_userRepository.IsSiteAdmin(details.ReviewerID))
                {
                    var offensiveContents = _offensiveContentRepository.GetItem(oc => oc.OffensiveContentID == details.EntryID);
                    if (offensiveContents != null)
                    {
                        offensiveContents.OffensiveStatusID = (int)details.Status;
                        offensiveContents.Justification = details.Justification;

                        offensiveContents.ReviewerID = details.ReviewerID;
                        offensiveContents.ReviewerDatetime = DateTime.UtcNow;

                        _offensiveContentRepository.Update(offensiveContents);
                        _offensiveContentRepository.SaveChanges();
                    }
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

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Updates the all the entries for the given Community with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if Community was updated; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public async Task<OperationStatus> UpdateAllOffensiveCommunityEntry(OffensiveEntry details)
        {
            this.CheckNotNull(() => new { details = details });

            OperationStatus status = null;
            try
            {
                if (_userRepository.IsSiteAdmin(details.ReviewerID))
                {
                    var offensiveCommunities =  _offensiveCommunitiesRepository.GetItems(oc => oc.CommunityID == details.EntityID && oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged, null, false);
                    if (offensiveCommunities != null && offensiveCommunities.Any())
                    {
                        foreach (var item in offensiveCommunities)
                        {
                            item.OffensiveStatusID = (int)details.Status;
                            item.Justification = details.Justification;

                            item.ReviewerID = details.ReviewerID;
                            item.ReviewerDatetime = DateTime.UtcNow;

                            _offensiveCommunitiesRepository.Update(item);
                        }

                        _offensiveCommunitiesRepository.SaveChanges();
                    }
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

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        /// <summary>
        /// Updates the all the entries for the given Content with all the details.
        /// </summary>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are returning the details of the exceptions as part of the function.")]
        public async Task<OperationStatus> UpdateAllOffensiveContentEntry(OffensiveEntry details)
        {
            this.CheckNotNull(() => new { details = details });

            OperationStatus status = null;
            try
            {
                if (_userRepository.IsSiteAdmin(details.ReviewerID))
                {
                    var offensiveContents =  _offensiveContentRepository.GetItems(oc => oc.ContentID == details.EntityID && oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged, null, false);
                    if (offensiveContents != null && offensiveContents.Any())
                    {
                        foreach (var item in offensiveContents)
                        {
                            item.OffensiveStatusID = (int)details.Status;
                            item.Justification = details.Justification;

                            item.ReviewerID = details.ReviewerID;
                            item.ReviewerDatetime = DateTime.UtcNow;

                            _offensiveContentRepository.Update(item);
                        }

                        _offensiveContentRepository.SaveChanges();
                    }
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

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        #endregion
    }
}
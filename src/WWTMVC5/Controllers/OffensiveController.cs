//-----------------------------------------------------------------------
// <copyright file="OffensiveController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Offensive partial view request which makes request to repository and gets/publishes the
    /// required data about Offensive and pushes them to the View.
    /// </summary>
    public class OffensiveController : ControllerBase
    {
        #region Member variables

        /// <summary>
        /// Instance of ReportEntity Service
        /// </summary>
        private IReportEntityService _reportEntityService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService _notificationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OffensiveController class.
        /// </summary>
        /// <param name="reportEntityService">Instance of reportEntity Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public OffensiveController(IReportEntityService reportEntityService, IProfileService profileService, INotificationService notificationService)
            : base(profileService)
        {
            _reportEntityService = reportEntityService;
            _notificationService = notificationService;
        }

        #endregion

        #region Action methods

        /// <summary>
        /// Save Action is the action that flags the entity as offensive
        /// </summary>
        /// <returns>Returns if the entity is flagged</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]//http://stackoverflow.com/questions/10851283/antiforgerytoken-deprecated-in-asp-net-mvc-4-rc
        public string ReportEntity(long entityId, EntityType entityType, string comments, ReportEntityType offenceType)
        {
            var status = string.Empty;
            if (!string.IsNullOrWhiteSpace(comments))
            {
                var report = new ReportEntityDetails()
                {
                    Comment = comments,
                    ReportEntityID = entityId,
                    ParentID = entityId,
                    Status = OffensiveStatusType.Flagged,
                    ReportedByID = CurrentUserId,
                    ReportEntityType = offenceType
                };

                switch (entityType)
                {
                    case EntityType.Community:
                    case EntityType.Folder:
                        _reportEntityService.ReportOffensiveCommunity(report);
                        break;
                    case EntityType.Content:
                        _reportEntityService.ReportOffensiveContent(report);
                        break;
                    default:
                        break;
                }

                // TODO: Only on succeeded we need to send the notification email.
                SendOffensiveEntityMail(report, entityType);
            }
            return status;
        }

        #endregion

        /// <summary>
        /// Send Join OffensiveEntity mail.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendOffensiveEntityMail(ReportEntityDetails details, EntityType entityType)
        {
            //// TODO: Need to send mail asynchronously. 

            try
            {
                // Send Mail.
                var request = new FlaggedRequest()
                {
                    ID = details.ReportEntityID,
                    EntityType = entityType,
                    ParentID = details.ParentID,
                    UserComments = details.Comment,
                    FlaggedOn = DateTime.UtcNow,
                    FlaggedAs = details.ReportEntityType.ToString(),
                    UserID = details.ReportedByID,
                    UserName = details.ReportedBy,
                    UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", GetBaseUrl(), details.ReportedByID),
                };

                switch (entityType)
                {
                    case EntityType.Community:
                    case EntityType.Folder:
                        request.Link = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", GetBaseUrl(), details.ReportEntityID);
                        break;
                    default:
                        request.Link = string.Format(CultureInfo.InvariantCulture, "{0}{1}/Index/{2}", GetBaseUrl(), entityType.ToString(), details.ReportEntityID);
                        break;
                }

                _notificationService.NotifyFlagged(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }
    }
}
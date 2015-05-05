//-----------------------------------------------------------------------
// <copyright file="StaticContentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq.Expressions;
using AutoMapper;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Static content Service having methods for retrieving Static Content
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class StaticContentService : IStaticContentService
    {
        #region Member variables

        /// <summary>
        /// Instance of Static Content repository
        /// </summary>
        private IRepositoryBase<StaticContent> staticContentRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the StaticContentService class.
        /// </summary>
        /// <param name="staticContentRepository">Instance of staticContent repository</param>
        public StaticContentService(IRepositoryBase<StaticContent> staticContentRepository)
        {
            this.staticContentRepository = staticContentRepository;
        }

        #endregion

        #region Pubic Methods

        /// <summary>
        /// Update Static Content
        /// </summary>
        /// <param name="staticContentDetails">static Content details.</param>
        /// <returns>True if the content was updated successfully; Otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        public OperationStatus UpdateStaticContent(StaticContentDetails staticContentDetails)
        {
            OperationStatus status = null;
            try
            {
                Expression<Func<StaticContent, bool>> condition = content => content.TypeID == (int)staticContentDetails.TypeID && content.IsDeleted == false;
                var contentValue = this.staticContentRepository.GetItem(condition);
                if (contentValue != null)
                {
                    contentValue.Content = staticContentDetails.Content;
                    contentValue.ModifiedByID = staticContentDetails.ModifiedByID;
                    contentValue.ModifiedDatetime = DateTime.UtcNow;
                    this.staticContentRepository.Update(contentValue);
                    this.staticContentRepository.SaveChanges();
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(string.Format(CultureInfo.CurrentCulture, Resources.StaticContentTypeNotFound, staticContentDetails.TypeID));
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
        /// Gets static content from the DB
        /// </summary>
        /// <param name="staticContentType">static Content Type</param>
        /// <returns>static content object</returns>
        public StaticContentDetails GetStaticContent(StaticContentType staticContentType)
        {
            Expression<Func<StaticContent, bool>> condition = (staticContent) => staticContent.StaticContentType.TypeID == (int)staticContentType && staticContent.IsDeleted == false;
            var content = this.staticContentRepository.GetItem(condition);

            var staticContentDetails = new StaticContentDetails();
            Mapper.Map(content, staticContentDetails);

            return staticContentDetails;
        }

        #endregion
    }
}
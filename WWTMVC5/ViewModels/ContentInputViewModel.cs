//-----------------------------------------------------------------------
// <copyright file="ContentInputViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using WWTMVC5.Properties;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the View Model which gets input for creating or updating a content.
    /// </summary>
    public class ContentInputViewModel : EntityInputBaseViewModel, IValidatableObject
    {
        /// <summary>
        /// Gets or sets the associated files
        /// </summary>
        public IEnumerable<HttpPostedFileBase> AssociatedFiles { get; set; }

        /// <summary>
        /// Gets or sets Video ID.
        /// </summary>
        public Guid VideoID { get; set; }

        /// <summary>
        /// Gets or sets Video Name.
        /// </summary>
        public string VideoName { get; set; }

        /// <summary>
        /// Gets or sets the associated video.
        /// </summary>
        public HttpPostedFileBase Video { get; set; }

        /// <summary>
        /// Gets or sets ContentData ID.
        /// </summary>
        public Guid ContentDataID { get; set; }

        /// <summary>
        /// Gets or sets the content fileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets content file details.
        /// </summary>
        public string ContentFileDetail { get; set; }

        /// <summary>
        /// Gets or sets the tour length value.
        /// </summary>
        public string TourLength { get; set; }

        /// <summary>
        /// Gets or sets the content data
        /// </summary>
        public HttpPostedFileBase ContentData { get; set; }

        /// <summary>
        /// Gets or sets the content URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is a Link or not.
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// Gets or sets Citation.
        /// </summary>
        [AllowHtml]
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets posted files.
        /// </summary>
        public IEnumerable<string> PostedFileName { get; set; }

        /// <summary>
        /// Gets or sets posted files details.
        /// </summary>
        public IEnumerable<string> PostedFileDetail { get; set; }

        /// <summary>
        /// Gets or sets posted video details.
        /// </summary>
        public string VideoFileDetail { get; set; }


        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.IsLink)
            {
                if (string.IsNullOrWhiteSpace(this.ContentUrl))
                {
                    yield return new ValidationResult(
                        string.Format(Resources.MissingMandatoryField, "ContentUrl"),
                        new string[] { "ContentUrl" });
                }
            }
            else if (ContentDataID == Guid.Empty)
            {
                yield return new ValidationResult(
                    string.Format(Resources.MissingMandatoryField, "ContentData"),
                    new string[] { "ContentData" });
            }
        }
    }
}
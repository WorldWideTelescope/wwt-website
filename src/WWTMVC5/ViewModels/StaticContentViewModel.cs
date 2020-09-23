//-----------------------------------------------------------------------
// <copyright file="StaticContentViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WWTMVC5.Properties;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the static content
    /// </summary>
    public class StaticContentViewModel
    {
        /// <summary>
        /// Initializes a new instance of the StaticContentViewModel class.
        /// </summary>
        public StaticContentViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the static content
        /// </summary>
        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the details about the page.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public int ContentType { get; set; }
    }
}
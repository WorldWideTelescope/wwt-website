//-----------------------------------------------------------------------
// <copyright file="EntityInputBaseViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WWTMVC5.Properties;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the Base View Model which gets input for creating or updating a community/folder/content.
    /// </summary>
    public abstract class EntityInputBaseViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the entity.
        /// </summary>
        public long? ID { get; set; }

        /// <summary>
        /// Gets or sets the Name of the entity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        [AllowHtml]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description of the entity.
        /// </summary>
        [AllowHtml]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Created by ID of the entity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public long CreatedByID { get; set; }

        /// <summary>
        /// Gets or sets the Owner ID of the entity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public long OwnerID { get; set; }

        /// <summary>
        /// Gets or sets the Distributed By  of the entity.
        /// </summary>
        [AllowHtml]
        public string DistributedBy { get; set; }

        /// <summary>
        /// Gets or sets the Category ID of the entity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the List of values for Categories.
        /// </summary>
        public SelectList CategoryList { get; set; }

        /// <summary>
        /// Gets or sets the Access Type ID of the entity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MissingMandatoryField")]
        public int AccessTypeID { get; set; }

        /// <summary>
        /// Gets or sets the Tags of the entity.
        /// </summary>
        [AllowHtml]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the Parent ID of the entity.
        /// </summary>
        public long? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the List of values for parent.
        /// </summary>
        public SelectList ParentList { get; set; }

        /// <summary>
        /// Gets or sets Thumbnail ID.
        /// </summary>
        public Guid ThumbnailID { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail link.
        /// </summary>
        public string ThumbnailLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is offensive or not.
        /// </summary>
        public bool IsOffensive { get; set; }

    }
}
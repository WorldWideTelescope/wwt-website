//-----------------------------------------------------------------------
// <copyright file="DefaultThumbnailViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the Default Thumbnail View which is used across the site.
    /// </summary>
    public class DefaultThumbnailViewModel
    {
        /// <summary>
        /// Initializes a new instance of the DefaultThumbnailViewModel class.
        /// </summary>
        /// <param name="thumbnailID">Thumbnail Guid</param>
        /// <param name="entity">Type of the entity</param>
        /// <param name="altText">Alt text for IMG tag</param>
        /// <param name="contentType">Content Type of the content. In case of community, it will be generic which will not be used.</param>
        public DefaultThumbnailViewModel(Guid? thumbnailID, EntityType entity, string altText, ContentTypes contentType)
        {
            this.ThumbnailID = thumbnailID;
            this.Entity = entity;
            this.AltText = HttpContext.Current.Server.HtmlEncode(altText);
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets or sets the ThumbnailID of the entity
        /// </summary>
        public Guid? ThumbnailID { get; set; }

        public string ThumbnailLink { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity
        /// </summary>
        public EntityType Entity { get; set; }

        /// <summary>
        /// Gets or sets the Alt text for the image tag
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        /// Gets or sets the content Type of the Content.
        /// </summary>
        public ContentTypes ContentType { get; set; }
    }
}
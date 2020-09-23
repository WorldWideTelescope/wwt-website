//-----------------------------------------------------------------------
// <copyright file="DeepZoomViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// View Model used in Pivot Viewer
    /// </summary>
    public class DeepZoomViewModel
    {
        /// <summary>
        /// Gets or sets the Community id or Content id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of content/community
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of content/community
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags of content/community
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the Last updated date of content/community
        /// </summary>
        public string LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the Distributor of content/community
        /// </summary>
        public string DistributedBy { get; set; }

        /// <summary>
        /// Gets or sets the category of the entity
        /// </summary>
        public CategoryType Category { get; set; }

        /// <summary>
        /// Gets or sets the rating of the entity
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the ThumbnailID of the entity
        /// </summary>
        public Guid? ThumbnailID { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity
        /// </summary>
        public EntityType Entity { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// In case of community, it represents the signup file name for the community.
        /// In case of content, it represents the file name of the content.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the Azure ID of the content. This will be only for Content.
        /// </summary>
        public Guid? ContentAzureID { get; set; }

        /// <summary>
        /// Gets or sets Citation.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets File Type. Applicable in case of Content.
        /// </summary>
        public ContentTypes FileType { get; set; }

        /// <summary>
        /// Gets or sets Link for Link Content.
        /// </summary>
        public string ContentLink { get; set; }

        /// <summary>
        /// Gets Download Link
        /// </summary>
        public string DownloadLink 
        {
            get
            {
                //// TODO : A Better way and place to do this
                //// Note that all the properties need to be set before invoking Download link property
                if (this.Entity == EntityType.Community)
                {
                    return "/Community/Signup/" + Id.ToString(CultureInfo.InvariantCulture);
                }
                else if (this.FileType == ContentTypes.Link)
                {
                    return ContentLink;
                }
                else
                {
                    return "/File/Download/" + ContentAzureID.ToString() + "/" + FileName;
                }
            }
        }
    }
}
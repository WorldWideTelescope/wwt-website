//-----------------------------------------------------------------------
// <copyright file="EntityViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web;
using WWTMVC5.Models;
using WWTMVC5.Properties;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the View for either a content or a community with all details
    /// to be shown in community/content top/latest/popular and inherited by community and content details.
    /// </summary>
    public class EntityViewModel
    {
        /// <summary>
        /// Private member for Name property
        /// </summary>
        private string name;

        /// <summary>
        /// Private member for Producer property
        /// </summary>
        private string producer;

        /// <summary>
        /// Private member for DistributedBy property
        /// </summary>
        private string distributedBy;

        /// <summary>
        /// Gets or sets the Community id or Content id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of content/community
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = HttpContext.Current.Server.HtmlDecode(value);
            }
        }

        /// <summary>
        /// Gets or sets the category of the entity
        /// </summary>
        public CategoryType Category { get; set; }

        /// <summary>
        /// Gets or sets the parent id of the entity
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent name of the entity
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent.
        /// </summary>
        public CommunityTypes ParentType { get; set; }

        /// <summary>
        /// Gets or sets the Tags of the entity
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the rating of the entity
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the number of people rated for the community/content
        /// </summary>
        public int RatedPeople { get; set; }

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
        /// Gets or sets the Azure ID of the content. Though this will be only for Content, since this
        /// view model is used by all places (latest/featured/popular/related/search, etc) only for this property,
        /// need not introduce one more model just to represent Content.
        /// </summary>
        public Guid? ContentAzureID { get; set; }

        /// <summary>
        /// Gets or sets the current user's permission on this entity.
        /// </summary>
        public Permission UserPermission { get; set; }

        /// <summary>
        /// Gets or sets the AccessType of the entity
        /// </summary>
        public AccessType AccessType { get; set; }

        /// <summary>
        /// Gets or sets the Producer of content/community
        /// </summary>
        public string Producer
        {
            get
            {
                return producer;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    producer = value;
                }
                else
                {
                    producer = Resources.DefaultProfileName;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Producer Id of content/community
        /// </summary>
        public long? ProducerId { get; set; }

        /// <summary>
        /// Gets or sets the content Type of the Content.
        /// </summary>
        public ContentTypes ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Distributor of content/community
        /// </summary>
        public string DistributedBy
        {
            get
            {
                return distributedBy;
            }
            set
            {
                distributedBy = HttpContext.Current.Server.HtmlDecode(value);
            }
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="Tour.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing a Tour of a Community.
    /// </summary>
    [Serializable]
    public class TourModel
    {
        /// <summary>
        /// Gets or sets the Title attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the MSRComponentId attribute value for the Tour.
        /// </summary>
        [XmlAttribute]
        public long MSRComponentId { get; set; }

        /// <summary>
        /// Gets or sets the description attribute value for the tour.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Author attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the OrganizationUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string OrganizationUrl { get; set; }

        /// <summary>
        /// Gets or sets the OrganizationName attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the TourUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string TourUrl { get; set; }

        /// <summary>
        /// Gets or sets the AuthorUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string AuthorURL { get; set; }

        /// <summary>
        /// Gets or sets the AuthorImageUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string AuthorImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the ThumbnailUrl attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the AverageRating attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string AverageRating { get; set; }

        /// <summary>
        /// Gets or sets the LengthInSecs attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string LengthInSecs { get; set; }

        /// <summary>
        /// Gets or sets the Permission details
        /// </summary>
        [XmlAttribute]
        public Permission Permission { get; set; }

        /// <summary>
        /// Gets or sets the RelatedTours attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string RelatedTours { get; set; }

        /// <summary>
        /// Gets or sets the TourDuration attribute value for the tour.
        /// </summary>
        [XmlAttribute]
        public string TourDuration { get; set; }
    }
}
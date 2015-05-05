//-----------------------------------------------------------------------
// <copyright file="Place.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing a place, i.e. links of a Community.
    /// </summary>
    [Serializable]
    public class Place
    {
        /// <summary>
        /// Gets or sets the MSRComponentId attribute value for the Place.
        /// </summary>
        [XmlAttribute]
        public long MSRComponentId { get; set; }

        /// <summary>
        /// Gets or sets the name attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail attribute value for the link.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets File Type. 
        /// </summary>
        [XmlIgnore]
        public ContentTypes FileType { get; set; }

        /// <summary>
        /// Gets or sets Link for Link Content.
        /// </summary>
        [XmlIgnore]
        public string ContentLink { get; set; }

        /// <summary>
        /// Gets or sets the Azure ID of the content. This will be only for Content.
        /// </summary>
        [XmlIgnore]
        public string ContentAzureID { get; set; }

        /// <summary>
        /// Gets or sets the Permission details
        /// </summary>
        [XmlAttribute]
        public Permission Permission { get; set; }
    }
}
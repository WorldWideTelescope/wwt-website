//-----------------------------------------------------------------------
// <copyright file="PayloadDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace WWTMVC5.Models
{
    [Serializable]
    [XmlRoot("Folder")]
    public class PayloadDetails
    {
        /// <summary>
        /// Gets or sets all child directories of the current folder.
        /// </summary>
        [XmlElement("Folder")]
        public Collection<PayloadDetails> Children { get; set; }

        /// <summary>
        /// Gets or sets the links present in the current folder.
        /// </summary>
        [XmlElement("Place")]
        public Collection<Place> Links { get; set; }

        /// <summary>
        /// Gets or sets the tours present in the current folder.
        /// </summary>
        [XmlElement("Tour")]
        public Collection<TourModel> Tours { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group of the folder.
        /// </summary>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the searchable property of the folder.
        /// </summary>
        [XmlAttribute]
        public string Searchable { get; set; }

        /// <summary>
        /// Gets or sets the Type property of the folder.
        /// </summary>
        [XmlAttribute("Type")]
        public string FolderType { get; set; }

        /// <summary>
        /// Gets or sets the Thumbnail path of the folder.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the folder.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Permission details
        /// </summary>
        [XmlAttribute]
        public Permission Permission { get; set; }

        /// <summary>
        /// Gets or sets the Id attribute value for the folder.
        /// </summary>
        [XmlIgnore]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the MSRCommunityId attribute value for the folder.
        /// </summary>
        [XmlAttribute]
        public long MSRCommunityId { get; set; }

        /// <summary>
        /// Gets or sets the MSRComponentId attribute value for the folder/WTML.
        /// </summary>
        [XmlAttribute]
        public long MSRComponentId { get; set; }

        /// <summary>
        /// Gets or sets the FolderRefreshType attribute value for the folder/WTML.
        /// </summary>
        [XmlAttribute, DefaultValue("")]
        public string FolderRefreshType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is a Folder or Collection.
        /// </summary>
        [XmlIgnore]
        public bool IsCollection { get; set; }

        /// <summary>
        /// Gets or sets the Type value for the folder (Visitor,Community, Folder, User).
        /// This will be used while providing the icons
        /// </summary>
        [XmlIgnore]
        public CommunityTypes CommunityType { get; set; }
    }
}
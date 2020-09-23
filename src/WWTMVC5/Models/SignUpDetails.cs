//-----------------------------------------------------------------------
// <copyright file="SignUpDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace WWTMVC5.Models
{
    [Serializable]
    [XmlRoot("Folder")]
    public class SignUpDetails
    {
        /// <summary>
        /// Gets or sets the name attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Group attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the Searchable attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Searchable { get; set; }

        /// <summary>
        /// Gets or sets the Type attribute value for the SignUp.
        /// </summary>
        [XmlAttribute("Type")]
        public string CommunityType { get; set; }

        /// <summary>
        /// Gets or sets the URL attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail attribute value for the SignUp.
        /// </summary>
        [XmlAttribute]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the MSRCommunityId attribute value for the folder.
        /// </summary>
        [XmlAttribute]
        public long MSRCommunityId { get; set; }
    }
}
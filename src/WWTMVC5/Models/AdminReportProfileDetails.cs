//-----------------------------------------------------------------------
// <copyright file="AdminReportProfileDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a User profile to be shown in reports page.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class AdminReportProfileDetails
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets PUID of the user.
        /// </summary>
        [DataMember]
        public string PUID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Email address of the user
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets JoinedDateTime date time.
        /// </summary>
        [DataMember]
        public DateTime? JoinedDateTime { get; set; }

        /// <summary>
        /// Gets or sets last login time.
        /// </summary>
        [DataMember]
        public DateTime? LastLoggedOn { get; set; }
        
        /// <summary>
        /// Gets or sets the total content uploaded by the user.
        /// </summary>
        [DataMember]
        public long TotalContents { get; set; }

        /// <summary>
        /// Gets or sets the last uploaded content date for the user.
        /// </summary>
        [DataMember]
        public DateTime? LastUploaded { get; set; }

        /// <summary>
        /// Gets or sets the total communities uploaded by the user.
        /// </summary>
        [DataMember]
        public long TotalCommunities { get; set; }

        /// <summary>
        /// Gets or sets the total used size for the user.
        /// </summary>
        [DataMember]
        public decimal TotalUsedSize { get; set; }
    }
}
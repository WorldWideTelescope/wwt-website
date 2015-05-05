//-----------------------------------------------------------------------
// <copyright file="AdminUserDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a AdminUser Details.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AdminUserDetails
    {
        /// <summary>
        /// Gets or sets name of user.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets ID of the user.
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// Gets or sets ID of the user image.
        /// </summary>
        [DataMember]
        public Guid? UserImageID { get; set; }

        /// <summary>
        /// Gets or sets EMAILof the user.
        /// </summary>
        [DataMember]
        public string Email { get; set; }
    }
}
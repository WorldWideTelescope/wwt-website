//-----------------------------------------------------------------------
// <copyright file="AdminUserDetailsList.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the List of AdminUserDetails.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AdminUserDetailsList
    {
        [DataMember]
        public Collection<AdminUserDetails> Users { get; set; }

        [DataMember]
        public Collection<AdminUserDetails> AdminUsers { get; set; }
    }
}
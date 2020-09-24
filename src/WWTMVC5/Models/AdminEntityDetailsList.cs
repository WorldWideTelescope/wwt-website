//-----------------------------------------------------------------------
// <copyright file="AdminEntityDetailsList.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the List of AdminEntity.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AdminEntityDetailsList
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        [DataMember]
        public Collection<AdminEntityDetails> Entities { get; set; }

        /// <summary>
        /// Gets or sets the featured entities.
        /// </summary>
        [DataMember]
        public Collection<AdminEntityDetails> FeaturedEntities { get; set; }
    }
}
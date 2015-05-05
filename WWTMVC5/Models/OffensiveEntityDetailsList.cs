//-----------------------------------------------------------------------
// <copyright file="OffensiveEntityDetailsList.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a OffensiveEntity.
    /// </summary>
    [DataContract(Namespace = "")]
    public class OffensiveEntityDetailsList
    {
        [DataMember]
        public Collection<OffensiveEntityDetails> Entities { get; set; }
    }
}
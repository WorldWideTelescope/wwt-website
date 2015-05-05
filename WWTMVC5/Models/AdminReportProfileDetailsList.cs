//-----------------------------------------------------------------------
// <copyright file="AdminReportProfileDetailsList.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the List of AdminReportProfileDetailsList.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AdminReportProfileDetailsList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Can't convert to a read only collection."), DataMember]
        public Collection<AdminReportProfileDetails> Users { get; set; }
    }
}
//-----------------------------------------------------------------------
// <copyright file="CommentFilter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about the filters needed while retrieving collection of comments.
    /// </summary>
    [Serializable]
    public class CommentFilter
    {
        /// <summary>
        /// Initializes a new instance of the CommentFilter class.
        /// </summary>
        /// <param name="orderType">Order type for the entities (Newest/Oldest?)</param>
        /// <param name="entityId">Id of the entity to which comment belongs to</param>
        public CommentFilter(OrderType orderType, long entityId)
        {
            this.OrderType = orderType;
            this.EntityId = entityId;
        }

        /// <summary>
        /// Gets or sets the highlight type (Featured/Latest/Popular)
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Gets or sets the entity id in case if the entities are retrieved for any specific entities.
        /// </summary>
        public long EntityId { get; set; }
    }
}
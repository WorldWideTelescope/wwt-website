//-----------------------------------------------------------------------
// <copyright file="MessagePayload.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Defines a generic payload for an Azure-based queue message.
    /// </summary>
    /// <remarks>
    /// If the message content is small, it can be embedded directly inside the
    /// MessagePayload instance. If the message content is large, then the content
    /// can be stored in a Blob and the MessagePayload instance carries a reference
    /// to the Blob.
    /// </remarks>
    [Serializable]
    public class MessagePayload
    {
        /// <summary>
        /// Initializes a new instance of the MessagePayload class.
        /// </summary>
        public MessagePayload() 
        {
        }

        /// <summary>
        /// Gets or sets the full name of the type serialized in the content of the payload.
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// Gets or sets the Blob address relative to its container.
        /// </summary>
        /// <remarks>
        /// This value if null or empty if the message content is embedded.
        /// </remarks>
        public string BlobAddress { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        /// <remarks>
        /// This is null if the content is passed via a Blob.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Can't covert to Collection nor can have method.")]
        public byte[] EmbeddedContent { get; set; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="EmailRequest.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Defines a message to trigger sending an e-mail message to a customer.
    /// </summary>
    [Serializable]
    public class EmailRequest
    {
        /// <summary>
        /// Initializes a new instance of the EmailRequest class.
        /// </summary>
        public EmailRequest()
        {
            this.Recipients = new List<MailAddress>();
        }

        /// <summary>
        /// Gets or sets the recipient's display names and email addresses.
        /// </summary>
        public List<MailAddress> Recipients { get; set; }

        /// <summary>
        /// Gets or sets the subject of the e-mail.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the e-mail.
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message body is HTML. If false, plain text is assumed.
        /// </summary>
        public bool IsHtml { get; set; }
    }
}

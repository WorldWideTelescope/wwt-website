//-----------------------------------------------------------------------
// <copyright file="Mail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WWTMVC5;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Class implements smtp mailing. 
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// Initializes a new instance of the Mail class.
        /// </summary>
        public Mail()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Mail class.
        /// </summary>
        /// <param name="isHtmlMail">Specifies whether the body contains html content.</param>
        public Mail(bool isHtmlMail)
        {
            this.From = null;
            this.Credentials = null;
            this.To = new List<MailAddress>();
            this.Cc = new List<MailAddress>();
            this.Bcc = new List<MailAddress>();
            this.ReplyTo = new List<MailAddress>();
            this.Subject = string.Empty;
            this.MessageBody = string.Empty;
            this.Attachments = new List<Attachment>();
            this.IsBodyHtml = isHtmlMail;
        }

        /// <summary>
        /// Gets or sets host credential.
        /// </summary>
        public ICredentialsByHost Credentials { get; set; }

        /// <summary>
        /// Gets or sets sender mail address.
        /// </summary>
        public MailAddress From { get; set; }

        /// <summary>
        /// Gets or sets receipient mail addresses.
        /// </summary>
        public List<MailAddress> To { get; set; }

        /// <summary>
        /// Gets or sets Cc mail addresses.
        /// </summary>
        public List<MailAddress> Cc { get; set; }

        /// <summary>
        /// Gets or sets Bcc mail addresses.
        /// </summary>
        public List<MailAddress> Bcc { get; set; }

        /// <summary>
        /// Gets or sets ReplyTo mail addresses.
        /// </summary>
        public List<MailAddress> ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets mail subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets mail body.
        /// </summary>
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the body of the message is in HTML format.
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// Gets or sets mail attachments.
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        public virtual bool Send()
        {
            MailMessage message = this.PrepareMail();
            SmtpClient smtpClient = new SmtpClient();
            return SendMail(smtpClient, message);
        }

        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        /// <param name="host">Specifies the smtp host server name.</param>
        public virtual void Send(string host)
        {
            MailMessage message = this.PrepareMail();
            SmtpClient smtpClient = new SmtpClient(host);
            SendMail(smtpClient, message);
        }

        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        /// <param name="host">Specifies the smtp host server name.</param>
        /// <param name="port">Specifies the smtp host server port.</param>
        public virtual void Send(string host, int port)
        {
            MailMessage message = this.PrepareMail();
            SmtpClient smtpClient = new SmtpClient(host, port);
            SendMail(smtpClient, message);
        }

        private bool SendMail(SmtpClient smtpClient, MailMessage message)
        {
            int retryCount = 0;
            bool resend = false, mailSent = false;

            do
            {
                try
                {
                    smtpClient.Send(message);
                    mailSent = true;
                    resend = false;
                }
                catch (NullReferenceException ex)
                {
                    // If Message body is null, then there is no point in retrying.
                    Logger.Logger.Error(ex, "Unexpected failure occurred in Mail.SendMail.");
                    resend = false;
                }
                catch (ArgumentNullException ex)
                {
                    // If Message body is null, then there is no point in retrying.
                    Logger.Logger.Error(ex, "Unexpected failure occurred in Mail.SendMail.");
                    resend = false;
                }
                catch (Exception ex)
                {
                    string toList = string.Join(",", this.To.Select(address => address.Address));
                    Logger.Logger.Error(ex, "Unexpected failure occurred while sending mail To:{0}, Subject:{1}, RetryCount = {2}", toList, this.Subject, retryCount);

                    // Set the resend flag if the number of retries have exceeded the maximum retry count.
                    resend = retryCount++ <= Constants.RetryCount;
                }
            }
            while (resend);

            return mailSent;
        }

        /// <summary>
        /// Prepares mail message for sending.
        /// </summary>
        /// <returns>Returns mail message object.</returns>
        private MailMessage PrepareMail()
        {
            MailMessage message = new MailMessage();
            message.IsBodyHtml = this.IsBodyHtml;

            // Add from Address
            if (this.From == null)
            {
                throw new Exception("From address is empty");
            }

            message.From = this.From;

            // Add To Addresses
            if (0 == this.To.Count)
            {
                throw new Exception("To address is empty");
            }

            this.To.ForEach((item) => message.To.Add(item));

            // Add Cc Addresses
            if (0 != this.Cc.Count)
            {
                this.Cc.ForEach((item) => message.CC.Add(item));
            }

            // Add Bcc Addresses
            if (0 != this.Bcc.Count)
            {
                this.Bcc.ForEach((item) => message.Bcc.Add(item));
            }

            // Add Cc Addresses
            if (0 != this.ReplyTo.Count)
            {
                this.ReplyTo.ForEach((item) => message.ReplyToList.Add(item));
            }

            // Add Subject 
            if (string.IsNullOrEmpty(this.Subject))
            {
                throw new Exception("Mail subject is empty");
            }

            message.Subject = this.Subject;

            // Add Mail Body
            message.Body = this.MessageBody;

            // Add all attachments
            if (0 != this.Attachments.Count)
            {
                this.Attachments.ForEach((item) => message.Attachments.Add(item));
            }

            return message;
        }
    }
}

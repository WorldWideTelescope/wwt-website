//-----------------------------------------------------------------------
// <copyright file="MultipleEmailTask.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using WWTMVC5;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Provides the ability to send e-mail to customers.
    /// </summary>
    public class MultipleEmailTask : ITask
    {
        /// <summary>
        /// Message request associated with this task.
        /// </summary>
        private object requests;

        public MultipleEmailTask(object request)
        {
            Debug.Assert(request != null, "email request is null");

            this.requests = request;
        }

        /// <summary>
        /// Method executes the mailing of the request.
        /// </summary>
        /// <param name="context">Specifies task processor.</param>
        public void Execute(TaskProcessor context)
        {
            try
            {
                this.DoExecute(context);
            }
            catch (Exception ex)
            {
                Logger.Logger.Error(ex, "Unexpected failure occurred in EmailTask.Execute.");
            }
        }

        /// <summary>
        /// Method used to send mail.
        /// </summary>
        /// <param name="request">Request details.</param>
        private static bool SendMail(object request)
        {
            int retryCount = 0;
            bool retry = false, mailSent = false;
            List<EmailRequest> emailRequest = new List<EmailRequest>();

            Logger.Logger.Info("Sending mails to recipients.");

            do
            {
                try
                {
                    emailRequest.UpdateFrom(request);

                    retry = false;
                }
                catch (Exception ex)
                {
                    Logger.Logger.Error(ex, "Unexpected failure occurred while retrieving mail details from database, RetryCount = {0} ", retryCount);

                    // Before retrying, wait for 5 seconds.
                    System.Threading.Thread.Sleep(5000);

                    // Set the resend flag if the number of retries have exceeded the maximum retry count.
                    retry = retryCount++ <= Constants.RetryCount;
                }
            }
            while (retry);

            foreach (var item in emailRequest)
            {
                try
                {
                    Mail mail = null;
                    if (item.IsHtml)
                    {
                        mail = new HtmlMail(true);
                    }
                    else
                    {
                        mail = new Mail();
                    }

                    Debug.Assert(!string.IsNullOrEmpty(Constants.SenderEmail), "sender email id is empty");
                    Debug.Assert(!string.IsNullOrEmpty(Constants.SenderDisplayName), "sender display name is empty");

                    Debug.Assert(!string.IsNullOrEmpty(Constants.ReplyToEmail), "reply to email id is empty");
                    Debug.Assert(!string.IsNullOrEmpty(Constants.ReplyToDisplayName), "reply to display name is empty");

                    Debug.Assert(!string.IsNullOrEmpty(Constants.SenderEmail), "sender name is empty");
                    Debug.Assert(!string.IsNullOrEmpty(Constants.SenderDisplayName), "sender display name is empty");

                    Debug.Assert(item != null, "request object is null");
                    Debug.Assert(item.Recipients != null, "recipients object is null");
                    Debug.Assert(item.Recipients.Count != 0, "no recipients found");
                    Debug.Assert(!string.IsNullOrEmpty(item.Subject), "mail subject is empty");
                    Debug.Assert(!string.IsNullOrEmpty(item.MessageBody), "mail body is empty");

                    mail.From = new MailAddress(Constants.SenderEmail, Constants.SenderDisplayName);
                    mail.ReplyTo.Add(new MailAddress(Constants.ReplyToEmail, Constants.ReplyToDisplayName));

                    mail.To.AddRange(item.Recipients);

                    // Need to Add BCC for all the mails.
                    mail.Bcc.Add(new MailAddress(Constants.BccEmail, Constants.BccEmail));

                    mail.Subject = item.Subject;
                    mail.MessageBody = item.MessageBody;

                    mailSent = mail.Send();
                    
                    // If one of the mail is not sent, break the loop and add the object back to the queue,
                    // so that retrying the mail will happen again after some time
                    if (!mailSent)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Swallowing exception in release build not to fail the task in case of mailing failure.
                    Logger.Logger.Error(ex, "Unexpected failure occurred while sending mail to recipients.");
#if DEBUG
                    throw ex;
#endif
                }
            }

            return mailSent;
        }

        /// <summary>
        /// Method executes the mailing of the request.
        /// </summary>
        /// <param name="context">Specifies task processor.</param>
        private void DoExecute(TaskProcessor context)
        {
            Debug.Assert(context != null, "context object is null");

            if (!Constants.EnableEmailing)
            {
                Logger.Logger.Info("Sending mails to recipients is disabled.");
                return;
            }

            if (!SendMail(this.requests))
            {
                // If mail is not sent, add the notification request back to the queue.
                this.requests.AddToNotificationQueue();
            }
        }
    }
}
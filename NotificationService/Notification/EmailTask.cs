//-----------------------------------------------------------------------
// <copyright file="EmailTask.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net.Mail;
using WWTMVC5;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Provides the ability to send e-mail to customers.
    /// </summary>
    public class EmailTask : ITask
    {
        /// <summary>
        /// Message request associated with this task.
        /// </summary>
        private object request;

        public EmailTask(object request)
        {
            Debug.Assert(request != null, "email request is null");

            this.request = request;
        }

        /// <summary>
        /// Method executes the email sending task.
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
        /// Method executes the email sending task.
        /// </summary>
        /// <param name="context">Specifies task processor.</param>
        private void DoExecute(TaskProcessor context)
        {
            Debug.Assert(context != null, "context object is null");

            //// TODO: Do we need to serialize the mail object before sending mail?

            if (!Constants.EnableEmailing)
            {
                Logger.Logger.Info("Sending mails to recipients is disabled.");
                return;
            }

            int retryCount = 0;
            bool retry = false, mailSent = false;
            EmailRequest emailRequest = new EmailRequest();

            Logger.Logger.Info("Sending mails to recipients.");

            do
            {
                try
                {
                    emailRequest.UpdateFrom(this.request);

                    retry = false;
                }
                catch (Exception ex)
                {
                    Logger.Logger.Error(ex, "Unexpected failure occurred while retrieving mail details from database, RetryCount = {0} ", retryCount);

                    // Set the resend flag if the number of retries have exceeded the maximum retry count.
                    retry = retryCount++ <= Constants.RetryCount;

                    // Before retrying, wait for 5 seconds.
                    System.Threading.Thread.Sleep(5000);

                    if (!retry)
                    {
                        // If there are not more retries, then set emailRequest as Null, so that the item will be added back to the queue
                        // so that retry to send mail will happen later.
                        emailRequest = null;
                    }
                }
            }
            while (retry);

            if (emailRequest != null)
            {
                try
                {
                    Mail mail = null;
                    if (emailRequest.IsHtml)
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

                    Debug.Assert(emailRequest != null, "request object is null");
                    Debug.Assert(emailRequest.Recipients != null, "recipients object is null");
                    Debug.Assert(emailRequest.Recipients.Count != 0, "no recipients found");
                    Debug.Assert(!string.IsNullOrEmpty(emailRequest.Subject), "mail subject is empty");
                    Debug.Assert(!string.IsNullOrEmpty(emailRequest.MessageBody), "mail body is empty");

                    mail.From = new MailAddress(Constants.SenderEmail, Constants.SenderDisplayName);
                    mail.ReplyTo.Add(new MailAddress(Constants.ReplyToEmail, Constants.ReplyToDisplayName));

                    mail.To.AddRange(emailRequest.Recipients);

                    // Need to Add BCC for all the mails.
                    mail.Bcc.Add(new MailAddress(Constants.BccEmail, Constants.BccEmail));

                    mail.Subject = emailRequest.Subject;
                    mail.MessageBody = emailRequest.MessageBody;

                    mailSent = mail.Send();

                    Logger.Logger.Info("Sending mails to recipients succeeded.");
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

            if (!mailSent)
            {
                // If mail is not sent, add the notification request back to the queue.
                this.request.AddToNotificationQueue();
            }
        }
    }
}

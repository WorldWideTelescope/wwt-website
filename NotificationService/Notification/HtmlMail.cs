//-----------------------------------------------------------------------
// <copyright file="HtmlMail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Class implements smtp mailing with Html body.
    /// </summary>
    public class HtmlMail : Mail
    {
        /// <summary>
        /// Initializes a new instance of the HtmlMail class.
        /// </summary>
        public HtmlMail()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HtmlMail class.
        /// </summary>
        /// <param name="containsCRLF">Specifies whether the content is text which contains newline chars.</param>
        public HtmlMail(bool containsCRLF)
            : base(true)
        {
            this.ContainsCRLF = containsCRLF;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the body of the message contains either \r\n or \n.
        /// </summary>
        public bool ContainsCRLF { get; set; }
        
        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        public override bool Send()
        {
            return base.Send();
        }

        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        /// <param name="host">Specifies the smtp host server name.</param>
        public override void Send(string host)
        {
            base.Send(host);
        }

        /// <summary>
        /// Sends the mail using smtp server.
        /// </summary>
        /// <param name="host">Specifies the smtp host server name.</param>
        /// <param name="port">Specifies the smtp host server port.</param>
        public override void Send(string host, int port)
        {
            base.Send(host, port);
        }

        /// <summary>
        /// Replaces all CR and NEWLINE characters from body text by an HTML line break character.
        /// </summary>
        private void ReplaceCRLF()
        {
            this.MessageBody = this.MessageBody.Replace("\r\n", "<br/>");
            this.MessageBody = this.MessageBody.Replace("\n", "<br/>");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Nop.Core.Domain.Messages;
using Twilio;

namespace Nop.Plugin.SMS.Twillio.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class SmsSender : ISmsSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        public void SendSms(string subject, string body,
            string fromNumber, string fromName, string toNumber, string toName,
             string replyToNumber = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            var client = new TwilioRestClient(Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Account_Sid, Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Auth_Token);
            client.SendMessage(fromNumber, toNumber, body);

           
        }

    }
}

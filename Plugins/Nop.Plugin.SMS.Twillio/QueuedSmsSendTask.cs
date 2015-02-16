using System;
using System.Net;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Plugin.SMS.Twillio.Services.Messages;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Core.Plugins;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Configuration;
using Nop.Services.Security;

namespace Nop.Plugin.SMS.Twillio
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    class QueuedSmsSendTask : ITask
    {
        private readonly IQueuedSmsService _queuedSmsService;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public QueuedSmsSendTask(IQueuedSmsService queuedSmsService,
            ISmsSender smsSender, ILogger logger)
        {
            this._queuedSmsService = queuedSmsService;
            this._smsSender = smsSender;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual void Execute()
        {
            var maxTries = 3;
            var queuedSmsList = _queuedSmsService.SearchSms(null,null, null, null,
                true, maxTries, false, 0, 500);
            foreach (var queuedSms in queuedSmsList)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedSms.Bcc) 
                            ? null 
                            : queuedSms.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedSms.CC) 
                            ? null 
                            : queuedSms.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    _smsSender.SendSms(queuedSms.Subject, 
                        queuedSms.Body,
                       queuedSms.From, 
                       queuedSms.FromName, 
                       queuedSms.To, 
                       queuedSms.ToName,
                       queuedSms.ReplyTo,
                       queuedSms.ReplyToName,
                       bcc, 
                       cc, 
                       queuedSms.AttachmentFilePath,
                       queuedSms.AttachmentFileName);

                    queuedSms.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    _logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedSms.SentTries = queuedSms.SentTries + 1;
                    _queuedSmsService.UpdateQueuedSms(queuedSms);
                }
            }
        }
    }
}

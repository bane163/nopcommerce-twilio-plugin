using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Plugin.SMS.Twillio.Domain;

namespace Nop.Plugin.SMS.Twillio.Services.Messages
{
    public partial interface IQueuedSmsService
    {
        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void InsertQueuedSms(QueuedSms queuedSms);

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void UpdateQueuedSms(QueuedSms queuedSms);

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void DeleteQueuedSms(QueuedSms queuedSms);

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        QueuedSms GetQueuedSmsById(int queuedSmslId);

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        IList<QueuedSms> GetQueuedSmsByIds(int[] queuedSmsIds);

        /// <summary>
        /// Search queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item collection</returns>
        IPagedList<QueuedSms> SearchSms(string fromNumber,
            string toNumber, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, int maxSendTries,
            bool loadNewest, int pageIndex, int pageSize);

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        void DeleteAllSms();
    }
}

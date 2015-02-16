using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Events;
using Nop.Plugin.SMS.Twillio.Domain;

namespace Nop.Plugin.SMS.Twillio.Services.Messages
{
    public partial class QueuedSmsService : IQueuedSmsService
    {
        private readonly IRepository<QueuedSms> _queuedSmsRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedSmsRepository">Queued sms repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public QueuedSmsService(IRepository<QueuedSms> queuedSmsRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext, 
            IDataProvider dataProvider, 
            CommonSettings commonSettings)
        {
            _queuedSmsRepository = queuedSmsRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a queued sms
        /// </summary>
        /// <param name="queuedSms">Queued sms</param>        
        public virtual void InsertQueuedSms(QueuedSms queuedSms)
        {
            if (queuedSms == null)
                throw new ArgumentNullException("queuedSms");

            _queuedSmsRepository.Insert(queuedSms);

            //event notification
            _eventPublisher.EntityInserted(queuedSms);
        }

        /// <summary>
        /// Updates a queued sms
        /// </summary>
        /// <param name="queuedSms">Queued sms</param>
        public virtual void UpdateQueuedSms(QueuedSms queuedSms)
        {
            if (queuedSms == null)
                throw new ArgumentNullException("queuedSms");

            _queuedSmsRepository.Update(queuedSms);

            //event notification
            _eventPublisher.EntityUpdated(queuedSms);
        }

        /// <summary>
        /// Deleted a queued sms
        /// </summary>
        /// <param name="queuedSms">Queued sms</param>
        public virtual void DeleteQueuedSms(QueuedSms queuedSms)
        {
            if (queuedSms == null)
                throw new ArgumentNullException("queuedSms");

            _queuedSmsRepository.Delete(queuedSms);

            //event notification
            _eventPublisher.EntityDeleted(queuedSms);
        }

        /// <summary>
        /// Gets a queued sms by identifier
        /// </summary>
        /// <param name="queuedSmsId">Queued sms identifier</param>
        /// <returns>Queued sms</returns>
        public virtual QueuedSms GetQueuedSmsById(int queuedSmsId)
        {
            if (queuedSmsId == 0)
                return null;

            return _queuedSmsRepository.GetById(queuedSmsId);

        }

        /// <summary>
        /// Get queued smss by identifiers
        /// </summary>
        /// <param name="queuedSmsIds">queued sms identifiers</param>
        /// <returns>Queued smss</returns>
        public virtual IList<QueuedSms> GetQueuedSmsByIds(int[] queuedSmsIds)
        {
            if (queuedSmsIds == null || queuedSmsIds.Length == 0)
                return new List<QueuedSms>();

            var query = from qe in _queuedSmsRepository.Table
                        where queuedSmsIds.Contains(qe.Id)
                        select qe;
            var queuedSmsList = query.ToList();
            //sort by passed identifiers
            var sortedQueuedSms = new List<QueuedSms>();
            foreach (int id in queuedSmsIds)
            {
                var queuedSms = queuedSmsList.Find(x => x.Id == id);
                if (queuedSms != null)
                    sortedQueuedSms.Add(queuedSms);
            }
            return sortedQueuedSms;
        }

        /// <summary>
        /// Gets all queued smss
        /// </summary>
        /// <param name="fromsms">From sms</param>
        /// <param name="tosms">To sms</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent smss</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued sms descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>sms item list</returns>
        public virtual IPagedList<QueuedSms> SearchSms(string fromSms,
            string toSms, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, int maxSendTries,
            bool loadNewest, int pageIndex, int pageSize)
        {
            fromSms = (fromSms ?? String.Empty).Trim();
            toSms = (toSms ?? String.Empty).Trim();
            
            var query = _queuedSmsRepository.Table;
            if (!String.IsNullOrEmpty(fromSms))
                query = query.Where(qe => qe.From.Contains(fromSms));
            if (!String.IsNullOrEmpty(toSms))
                query = query.Where(qe => qe.To.Contains(toSms));
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = query.OrderByDescending(qe => qe.Priority);
            query = loadNewest ? 
                ((IOrderedQueryable<QueuedSms>)query).ThenByDescending(qe => qe.CreatedOnUtc) :
                ((IOrderedQueryable<QueuedSms>)query).ThenBy(qe => qe.CreatedOnUtc);

            var queuedSmsList = new PagedList<QueuedSms>(query, pageIndex, pageSize);
            return queuedSmsList;
        }

        /// <summary>
        /// Delete all queued smss
        /// </summary>
        public virtual void DeleteAllSms()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                //TODO: do not hard-code the table name
                _dbContext.ExecuteSqlCommand("TRUNCATE TABLE [QueuedSms]");
            }
            else
            {
                var queuedSmsList = _queuedSmsRepository.Table.ToList();
                foreach (var qe in queuedSmsList)
                    _queuedSmsRepository.Delete(qe);
            }
        }
    }
}

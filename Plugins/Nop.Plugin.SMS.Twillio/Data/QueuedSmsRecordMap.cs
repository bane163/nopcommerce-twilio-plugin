using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.SMS.Twillio.Domain;

namespace Nop.Plugin.SMS.Twillio.Data
{
    public partial class QueuedSmsRecordMap : EntityTypeConfiguration<QueuedSms>
    {
        public QueuedSmsRecordMap()
        {
            this.ToTable("QueuedSmsMessage");
            this.HasKey(x => x.Id);
        }
    }
}
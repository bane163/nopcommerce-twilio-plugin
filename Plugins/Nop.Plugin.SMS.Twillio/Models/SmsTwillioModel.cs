using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Twillio.Models
{
    public class SmsTwillioModel
    {
        [NopResourceDisplayName("Plugins.Sms.Twillio.Fields.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Twillio.Fields.SmsNumber")]
        public string SmsNumber { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Twillio.Fields.TestMessage")]
        public string TestMessage { get; set; }
        public string TestSmsResult { get; set; }
    }
}

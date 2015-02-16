using Nop.Core.Configuration;

namespace Nop.Plugin.SMS.Twillio
{
    public class TwillioSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the value indicting whether this SMS provider is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the Verizon email
        /// </summary>
        public string SmsNumber { get; set; }
    }
}
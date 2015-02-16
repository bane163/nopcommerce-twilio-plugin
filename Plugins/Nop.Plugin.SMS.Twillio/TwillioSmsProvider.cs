using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Plugin.SMS.Twillio.Services.Messages;
using Nop.Plugin.SMS.Twillio.Data;
using Nop.Plugin.SMS.Twillio.Domain;
using Nop.Core.Data;
using Twilio;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;

namespace Nop.Plugin.SMS.Twillio
{
    /// <summary>
    /// Represents the Twillio SMS provider
    /// </summary>
    public class TwillioSmsProvider : BasePlugin, IMiscPlugin
    {
        private readonly QueuedSmsObjectContext _context;
        private readonly IRepository<QueuedSms> _smsRepo;
        private readonly TwillioSettings _twillioSettings;
        private readonly IQueuedSmsService _queuedSmsService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;

        public TwillioSmsProvider(QueuedSmsObjectContext context, IRepository<QueuedSms> smsRepo, TwillioSettings twillioSettings,
            IQueuedSmsService queuedSmsService, ILogger logger, ISettingService settingService, IScheduleTaskService scheduleTaskService)
        {
            this._context = context;
            this._smsRepo = smsRepo;
            this._twillioSettings = twillioSettings;
            this._queuedSmsService = queuedSmsService;
            this._logger = logger;
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public bool SendSms(string text)
        {
            try
            {
                //var client = new TwilioRestClient(Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Account_Sid, Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Auth_Token);                
                //client.SendMessage(Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Trial_Number, _twillioSettings.SmsNumber, "Test Message");
                var queuedSms = new QueuedSms()
                {
                    Priority = 5,
                    From = Properties.Settings.Default.Nop_Plugin_SMS_Twillio_Trial_Number,
                    To = _twillioSettings.SmsNumber,
                    Body = "test",
                    CreatedOnUtc = DateTime.UtcNow
                };

                _queuedSmsService.InsertQueuedSms(queuedSms);

                //var SmsAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                //if (emailAccount == null)
                //    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                //if (emailAccount == null)
                //    throw new Exception("No email account could be loaded");

                //var queuedEmail = new QueuedEmail()
                //{
                //    Priority = 5,
                //    From = emailAccount.Email,
                //    FromName = emailAccount.DisplayName,
                //    To = _twillioSettings.SmsNumber,
                //    ToName = string.Empty,
                //    Subject = _storeContext.CurrentStore.Name,
                //    Body = text,
                //    CreatedOnUtc = DateTime.UtcNow,
                //    EmailAccountId = emailAccount.Id
                //};

                //_queuedSmsService.InsertQueuedEmail(queuedEmail);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SmsTwillio";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.SMS.Twillio.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _context.Install();
            //settings
            var settings = new TwillioSettings()
            {
                SmsNumber = "8888888888",
            };
            _settingService.SaveSetting(settings);

            

            //locales
            this.AddOrUpdatePluginLocaleResource("Admin.Common.SendSms.Selected", "Send SMS(Selected)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.TestFailed", "Test message sending failed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.TestSuccess", "Test message was sent (queued)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.Enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.Enabled.Hint", "Check to enable SMS provider");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.Sms", "Sms");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.Sms.Hint", "Twillio Sms address(e.g. your_phone_number@vtext.com)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.TestMessage", "Message text");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.TestMessage.Hint", "Text of the test message");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.SendTest", "Send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.SendTest.Hint", "Send test message");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Twillio.Fields.SmsNumber", "To Number");

            

            InstallScheduleTask();
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();
            //settings
            _settingService.DeleteSetting<TwillioSettings>();

            //locales
            this.DeletePluginLocaleResource("Admin.Common.SendSms.Selected");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.TestSuccess");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.Sms");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.Sms.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.TestMessage");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.TestMessage.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.SendTest");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.SendTest.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Twillio.Fields.SmsNumber");
            


            var task = FindScheduledTask();
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            base.Uninstall();
        }

        private void InstallScheduleTask()
        {
            //Check the database for the task
            var task = FindScheduledTask();

            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Send SMS",
                    //each 60 minutes
                    Seconds = 60,
                    Type = "Nop.Plugin.SMS.Twillio.QueuedSmsSendTask, Nop.Plugin.SMS.Twillio",
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }
        }

        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.SMS.Twillio.QueuedSmsSendTask, Nop.Plugin.SMS.Twillio");
        }
    }
}

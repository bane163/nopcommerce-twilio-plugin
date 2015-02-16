using System;
using System.Web.Mvc;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Twillio;
using Nop.Plugin.SMS.Twillio.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.SMS.Twillio.Models;
using System.Reflection;

namespace Nop.Plugin.SMS.Twillio.Controllers
{
    [AdminAuthorize]
    public class SmsTwillioController : BasePluginController
    {
        private readonly TwillioSettings _twillioSettings;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;

        public SmsTwillioController(TwillioSettings twillioSettings,
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService)
        {
            this._twillioSettings = twillioSettings;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
        }

        public ActionResult Manage()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new SmsTwillioModel();
            model.Enabled = _twillioSettings.Enabled;
            model.SmsNumber = _twillioSettings.SmsNumber;
            return View(model);
            
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(SmsTwillioModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _twillioSettings.Enabled = model.Enabled;
            _twillioSettings.SmsNumber = model.SmsNumber;
            _settingService.SaveSetting(_twillioSettings);

            return Configure();
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test-sms")]
        public ActionResult TestSms(SmsTwillioModel model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.TestMessage))
                {
                    model.TestSmsResult = "Enter test message";
                }
                else
                {
                    var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("SMS.Twillio");
                    if (pluginDescriptor == null)
                        throw new Exception("Cannot load the plugin");
                    var plugin = pluginDescriptor.Instance() as TwillioSmsProvider;
                    if (plugin == null)
                        throw new Exception("Cannot load the plugin");

                    if (!plugin.SendSms(model.TestMessage))
                    {
                        model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Twillio.TestFailed");
                    }
                    else
                    {
                        model.TestSmsResult = _localizationService.GetResource("Plugins.Sms.Twillio.TestSuccess");
                    }
                }
            }
            catch(Exception exc)
            {
                model.TestSmsResult = exc.ToString();
            }

            return View(model);
        }
    }

      
    }

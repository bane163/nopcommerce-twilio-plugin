using Autofac;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Data;
using Nop.Core.Data;
using Autofac.Core;
using Nop.Plugin.SMS.Twillio.Services.Messages;
using Nop.Plugin.SMS.Twillio.Data;
using Nop.Plugin.SMS.Twillio.Domain;
using Twilio;

namespace Nop.Plugin.SMS.Twillio.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_sms_twillio";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<SmsSender>().As<ISmsSender>().InstancePerLifetimeScope();                        
            builder.RegisterType<QueuedSmsService>().As<IQueuedSmsService>().InstancePerLifetimeScope();            
            this.RegisterPluginDataContext<QueuedSmsObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<QueuedSms>>()
                .As<IRepository<QueuedSms>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

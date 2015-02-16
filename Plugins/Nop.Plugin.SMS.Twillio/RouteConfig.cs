using Nop.Plugin.SMS.Twillio.Infrastructure;
using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.SMS.Twillio
{
    public class RouteConfig : IRouteProvider
    {
        public int Priority
        {
            get { return 0; }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.SMS.Twillio.ManageSms",
                "Twillio/Manage",
                new { controller = "SmsTwillio", action = "Manage" },
                new[] { "Nop.Plugin.SMS.Twillio.Controllers" }
           );

            routes.MapRoute("Plugin.SMS.Twillio.ConfigureSms",
                "Twillio/SmsTwillio/Configure",
                new { controller = "SmsTwillio", action = "Configure" },
                new[] { "Nop.Plugin.SMS.Twillio.Controllers" }
           );


            ViewEngines.Engines.Insert(0, new CustomViewEngine());
            //ViewEngines.Engines.Add(new CustomViewEngine());
        }
    }
}


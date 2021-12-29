using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KEN
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                // baans change 28th June for Login
                //defaults: new { controller = "Opportunity", action = "getOpportunitylist", id = UrlParameter.Optional }
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
                // baans end 28th June
            );
        }
    }
}

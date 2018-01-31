using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KBVault.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "PageListById",
              url: "{controller}/{action}/{id}/{page}",
              defaults: new { controller = "Home" });

            routes.MapRoute(
                name: "PageList",
                url: "{controller}/List/{page}",
                defaults: new { controller = "Home", action="List" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
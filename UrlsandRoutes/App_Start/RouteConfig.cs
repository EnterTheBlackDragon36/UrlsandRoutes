using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UrlsandRoutes.Infrastructure;

namespace UrlsandRoutes
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.Add(new Route("SayHello", new CustomRouteHandler()));

            routes.Add(new LegacyRoute("~/articles/Windows_3.1_Overview.html", "~/old/.NET_1.0_Class_Library"));
             
            //routes.MapRoute("NewRoute", "App/Do{action}",
            //    new { controller = "Home" });

            //These statements have been commented out
            

            //routes.MapRoute("MyRoute", "{controller}", "{action}");
            //routes.MapRoute("MyOtherRoute", "App/{action}", new { controller = "Home" });

            //routes.MapRoute("MyRoute", "{controller}/{action}/{id}/{*catchall}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute("MyRoute", "{controller}/{action}", null, new[] { "UrlsAndRoutes.Controllers" });
            routes.MapRoute("MyOtherRoute", "App/{action}", new { controller = "Home" }, new[] { "UrlsAndRoutes.Controllers" });
                

        }
    }
}
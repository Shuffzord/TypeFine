using System.Web.Mvc;
using System.Web.Routing;

namespace Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Pobierz",
                url: "Pobierz", 
                defaults:   new { controller = "Home", action = "Get" }
            );

            routes.MapRoute(
               name: "Main",
               url: "{q}",
               defaults: new { controller = "Home", action = "Index", q = UrlParameter.Optional }
           ); 

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}

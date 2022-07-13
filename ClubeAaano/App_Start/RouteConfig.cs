using System.Web.Mvc;
using System.Web.Routing;

namespace ClubeAaanoSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Inicio",
               url: "{controller}/{action}",
               defaults: new { controller = "Usuario", action = "Inicio" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Usuario", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

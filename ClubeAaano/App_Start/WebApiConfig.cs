using System.Web.Http;
using System.Web.Http.Cors;

namespace ClubeAaanoSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // EnableCorsAttribute cors = new EnableCorsAttribute(origin, "*", "GET,POST");
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Controllers with Actions
            // To handle routes like `/api/VTRouting/route`
            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/{controller}/{action}");

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
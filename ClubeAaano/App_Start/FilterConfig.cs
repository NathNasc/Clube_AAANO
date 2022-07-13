using System.Web;
using System.Web.Mvc;

namespace ClubeAaanoSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            if (HttpContext.Current.IsCustomErrorEnabled)
            {
                filters.Add(new RequireHttpsAttribute());

                //Verifica no WebConfig se é local ou servidor
                if (HttpContext.Current.IsCustomErrorEnabled)
                {
                    filters.Add(new RequireHttpsAttribute());
                }
            }

        }
    }
}

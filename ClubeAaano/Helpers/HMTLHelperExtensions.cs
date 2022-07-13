using System;
using System.Web.Mvc;

namespace ClubeAaanoSite
{
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (currentController == "AssinaturaPagSeguro" || currentAction == "ResgatesRegistrados")
            {
                currentController = "Assinatura";
            }

            if (currentController == "LojaParceira" || currentController == "PlanoPagSeguro"
               || (currentController == "Usuario" && currentAction == "Index"))
            {
                currentController = "Cadastros";
            }

            if(currentController == "EmailEnviado")
            {
                currentController = "ModeloEmail";
            }

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

    }
}
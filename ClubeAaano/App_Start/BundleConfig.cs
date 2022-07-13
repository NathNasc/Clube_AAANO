using System.Web.Optimization;

namespace ClubeAaanoSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            string components = "~/Scripts/ClubeAaanoSite/components/";
            string plugins = "~/Scripts/ClubeAaanoSite/plugins/";
            string js = "~/Scripts/ClubeAaanoSite/js/";
            string telas = "~/Scripts/ClubeAaanoSite/Telas/";
            string css = "~/Content/ClubeAaanoSite/css/";

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include(plugins + "jQueryUI/jquery-3.3.1.js"));
                
            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include(plugins + "jQueryUI/jquery-ui.min.js")
                .Include(plugins + "bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js")
                .Include(plugins + "jQueryUI/toastr.min.js")
                .Include(plugins + "jQueryUI/jquery.mask.min.js")
                .Include(components + "bootstrap/dist/js/bootstrap.min.js")
                .Include(js + "ClubeAaanoSite.min.js")
                .Include(js + "SweetAlert.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/inicio")
                .Include(components + "raphael/raphael.min.js")
                .Include(components + "morris.js/morris.min.js")
                .Include(components + "chart.js/Chart.min.js")
                .Include(components + "jquery-knob/dist/jquery.knob.min.js")
                .Include(components + "jquery-slimscroll/jquery.slimscroll.min.js")
                .Include(telas + "Inicio.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include(components + "bootstrap/dist/css/bootstrap.min.css")
                .Include("~/Content/jQuery/toastr.css")
                .Include(components + "font-awesome/css/font-awesome.min.css")
                .Include(plugins + "bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css")
                .Include(css + "Personalizado.css")
                .Include(css + "AdminLTE.min.css")
                .Include(css + "skins/_all-skins.min.css")
                .Include(components + "morris.js/morris.css")
                .Include(components + "jvectormap/jquery-jvectormap.css"));
            

        }
    }
}

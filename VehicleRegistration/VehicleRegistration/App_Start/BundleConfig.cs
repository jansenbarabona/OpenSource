using System.Web;
using System.Web.Optimization;


namespace VehicleRegistration
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                                   "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                    "~/Scripts/Admin_temp/login/js/bootstrap.min.js",
                    "~/Scripts/Admin_temp/toastr/toastr.min.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Admin/css").Include(
                      "~/Scripts/Admin_temp/dist/css/adminlte.min.css",
                      "~/Scripts/Admin_temp/icheck-bootstrap/icheck-bootstrap.min.css",
                      "~/scripts/Admin_temp/toastr/toastr.min.css",
                      "~/scripts/Admin_temp/select2/css/select2.min.css",
                      "~/scripts/Admin_temp/select2-bootstrap4-theme/select2-bootstrap4.min.css",
                      "~/Scripts/Admin_temp/datatables-bs4/css/dataTables.bootstrap4.min.css"
                      )
                      //.Include
                      //("~/Scripts/Admin_temp/fontawesome-free/css/all.min.css", new CssRewriteUrlTransform())
                      );
            bundles.Add(new ScriptBundle("~/Admin/js").Include(
                    "~/Scripts/Admin_temp/jquery/jquery.min.js",
                    "~/Scripts/Admin_temp/datatables/jquery.dataTables.min.js",
                    "~/Scripts/Admin_temp/datatables-bs4/js/dataTables.bootstrap4.min.js",
                    "~/Scripts/Admin_temp/datatables-buttons/js/dataTables.buttons.min.js",
                    "~/scripts/Admin_temp/jquery-ui/jquery-ui.min.js",
                    "~/Scripts/Admin_temp/bootstrap/js/bootstrap.bundle.min.js",
                    "~/Scripts/Admin_temp/dist/js/adminlte.min.js",
                    "~/Scripts/Admin_temp/dist/js/demo.min.js",
                    "~/Scripts/Admin_temp/overlayScrollbars/js/jquery.overlayScrollbars.min.js",
                    "~/Scripts/Admin_temp/jquery-mousewheel/jquery.mousewheel.min.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                      //"~/Content/materialize.css",
                      //"~/Content/site.css",
                      //"~/Content/PagedList.css",
                      //"~/Content/jquery-ui.css",
                      "~/scripts/Admin_temp/toastr/toastr.min.css",
                      "~/scripts/Login_temp/"
                      ));
        }
    }
}
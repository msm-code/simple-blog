using System.Web;
using System.Web.Optimization;

namespace SimpleBlog {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/js/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/js/app").Include(
                      "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                      "~/content/themes/simple/style.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/content/admin").Include(
                      "~/content/themes/simple/admin.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/content/jqgrid")
                      .Include("~/Scripts/jqgrid/css/ui.jqgrid.css",new CssRewriteUrlTransform())
                      .Include("~/Content/themes/simple/jqueryuicustom/css/sunny/jquery-ui-1.9.2.custom.min.css", new CssRewriteUrlTransform()));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}

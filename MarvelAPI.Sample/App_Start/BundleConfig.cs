using System.Web;
using System.Web.Optimization;

namespace MarvelAPI.Sample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Ember bundle
            bundles.Add(new ScriptBundle("~/bundles/ember")
                .Include("~/Scripts/handlebars-1.1.2.js")
                .Include("~/Scripts/ember-1.4.0.js")
                .Include("~/Scripts/ember-data.js"));

            //bundles.Add(new ScriptBundle("~/bundles/app")
            //    .Include("~/Scripts/app.js")
            //    .Include("~/Scripts/store.js")
            //    .Include("~/Scripts/router.js")
            //    .IncludeDirectory("~/Scripts/models", "*.js")
            //    .IncludeDirectory("~/Scripts/routes", "*.js"));
        }
    }
}

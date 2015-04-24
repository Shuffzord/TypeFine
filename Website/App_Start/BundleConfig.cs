using System.Web.Optimization;

namespace Website
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
               "~/Scripts/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/Services")
                   .IncludeDirectory("~/Scripts/Services", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Controllers")
                        .IncludeDirectory("~/Scripts/Controllers", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Engine")
                      .IncludeDirectory("~/Scripts/Engine", "*.js", true));

        }
    }
}
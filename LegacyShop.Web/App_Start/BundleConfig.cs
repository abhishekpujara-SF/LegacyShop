using System.Web.Optimization;

namespace LegacyShop.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/legacy-ui.css",
                "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/Scripts/site.js"));
        }
    }
}

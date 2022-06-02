using System.Web;
using System.Web.Optimization;

namespace AIRMAM5
{
    public class BundleConfig
    {
        //todo 前端-css bundle
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*_Layout.cshtml使用的semantic ui js*/
            bundles.Add(new ScriptBundle("~/bundles/LayoutSemantic").Include(
               "~/Content/semanticUI/js/sidebar.min.js",
               "~/Content/semanticUI/js/accordion.min.js",
               "~/Content/semanticUI/js/checkbox.min.js",
               "~/Content/semanticUI/js/dimmer.js",
               "~/Content/semanticUI/js/dropdown.min.js",
               "~/Content/semanticUI/js/transition.min.js",
               "~/Content/semanticUI/js/modal.min.js",
               "~/Content/semanticUI/js/popup.min.js",
               "~/Content/semanticUI/js/tab.min.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}

using System.Web;
using System.Web.Optimization;

namespace KBVault.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            CssRewriteUrlTransform relativeCssTrans = new CssRewriteUrlTransform();

            bundles.Add(new ScriptBundle("~/public/js")
                .Include("~/Assets/js/jquery/jquery-2-0-2.js",relativeCssTrans )
                .Include("~/Assets/js/jquery/jquery-ui-1-10-4.js",relativeCssTrans)
                .Include("~/Assets/js/bootstrap/bootstrap.js",relativeCssTrans)
                .Include("~/Assets/js/plugins/cookie/jquery.cookie.js",relativeCssTrans)
                .Include("~/Assets/js/frontend.js", relativeCssTrans)
                .Include("~/Assets/js/smartmenus/jquery.smartmenus.bootstrap.js",relativeCssTrans)
                .Include("~/Assets/js/smartmenus/jquery.smartmenus.js", relativeCssTrans )
                .Include("~/Assets/js/smartmenus/jquery.smartmenus.keyboard.js", relativeCssTrans )
            );

           
            

            bundles.Add( new StyleBundle("~/public/css").Include(
                "~/Assets/css/plugins/bootstrap/css/bootstrap.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/fontawesome/css/font-awesome.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/ionicons/css/ionicons.css", relativeCssTrans)
                .Include("~/Assets/css/site.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/smartmenus/jquery.smartmenus.bootstrap.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/jquery-ui/jquery-ui-redmond.css", relativeCssTrans)                
                );                                               

            bundles.Add(new ScriptBundle("~/admin/js").Include(
                "~/Assets/js/jquery/jquery-2-0-2.js",
                "~/Assets/js/jquery/jquery-ui-1-10-4.js",
                "~/Assets/js/bootstrap/bootstrap.js",
                "~/Assets/js/plugins/x-editable/xeditable.js",
                "~/Assets/js/AdminLTE/app.js",
                "~/Assets/ckeditor/ckeditor.js",
                "~/Assets/js/plugins/tag-it/tag-it.js",
                "~/Assets/datatables/js/jquery.dataTables.js",
                "~/Assets/js/plugins/uploader/jquery.uploadfile.js",
                "~/Assets/js/kbvault.js"
                ));
            
            bundles.Add(new StyleBundle("~/admin/css")
                .Include("~/Assets/css//plugins/bootstrap/css/bootstrap.css", relativeCssTrans)
                .Include("~/Assets/css/jquery-ui-redmond.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/fontawesome/css/font-awesome.css", relativeCssTrans)
                .Include("~/Assets/css/plugins/ionicons/css/ionicons.css", relativeCssTrans)
                .Include("~/Assets/css/AdminLTE.css")
                .Include("~/Assets/css/plugins/tagit/jquery.tagit.css",relativeCssTrans )
                .Include("~/Assets/css/plugins/tagit/tagit.ui-zendesk.css", relativeCssTrans)
                .Include("~/Assets/css/uploadfile.css",relativeCssTrans)
                .Include("~/Assets/css/plugins/xeditable/xeditable.css",relativeCssTrans)
                .Include("~/Assets/datatables/css/jquery.dataTables.css",relativeCssTrans)
                .Include("~/Assets/datatables/css/jquery.dataTables_themeroller.css",relativeCssTrans)
                .Include("~/Assets/datatables/css/jquery.datatables.bootstrap.css",relativeCssTrans)
                );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            /*
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            */
            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            
        }
    }
}
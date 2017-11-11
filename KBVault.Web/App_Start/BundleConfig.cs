using System.Web;
using System.Web.Optimization;

namespace KBVault.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {                        
            bundles.Add(new ScriptBundle("~/admin/js").Include(
                "~/Assets/js/jquery/jquery-2-0-2.js",
                "~/Assets/js/jquery/jquery-ui-1-10-4.js",
                "~/Assets/js/bootstrap/bootstrap.js",
                "~/Assets/js/plugins/x-editable/xeditable.js",
                "~/Assets/js/plugins/jqueryForm/jquery.form.js",
                "~/Assets/js/plugins/slug-it/jquery.slugit.js",                                
                "~/Assets/js/plugins/tag-it/tag-it.js",
                "~/Assets/datatables/js/jquery.dataTables.js",
                "~/Assets/js/plugins/uploader/jquery.uploadfile.js",
                "~/Assets/js/AdminLTE/app.js",
                "~/Assets/ckeditor/ckeditor.js",
                "~/Scripts/jquery.signalR-2.2.1.js",
                "~/Assets/js/kbvault.js"
                ));
            
            bundles.Add(new StyleBundle("~/admin/css").Include(
                "~/Assets/css/plugins/bootstrap.css",
                "~/Assets/css/plugins/font-awesome.css",                
                "~/Assets/css/plugins/ionicons.css",
                "~/Assets/css/plugins/xeditable/xeditable.css",
                "~/Assets/css/plugins/tagit/jquery.tagit.css",
                "~/Assets/css/plugins/tagit/tagit.ui-zendesk.css",               
                "~/Assets/css/uploadfile.css",                                
                "~/Assets/datatables/css/jquery.dataTables.css",
                "~/Assets/datatables/css/jquery.dataTables_themeroller.css",
                "~/Assets/datatables/css/jquery.datatables.bootstrap.css",
                "~/Assets/css/jquery-ui/jquery-ui-redmond.css",
                "~/Assets/css/AdminLTE.css"
                ));
            
        }
    }
}
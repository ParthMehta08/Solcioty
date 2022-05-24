using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Solcioty
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Style Bundles

            bundles.Add(new StyleBundle("~/bundle/bootstrapcss").Include("~/Content/assets/css/bootstrap.min.css",
                                                                        "~/Content/plugins/bootstrap-tagsinput/css/bootstrap-tagsinput.css"));
            bundles.Add(new StyleBundle("~/bundle/corecss").Include("~/Content/assets/css/core.css"));
            bundles.Add(new StyleBundle("~/bundle/components").Include("~/Content/assets/css/components.css"));
            bundles.Add(new StyleBundle("~/bundle/icons").Include("~/Content/assets/css/icons.css"));
            bundles.Add(new StyleBundle("~/bundle/pages").Include("~/Content/assets/css/pages.css"));
            bundles.Add(new StyleBundle("~/bundle/menu").Include("~/Content/assets/css/menu.css"));
            bundles.Add(new StyleBundle("~/bundle/responsive").Include("~/Content/assets/css/responsive.css"));
            //bundles.Add(new StyleBundle("~/bundle/jqueryui").Include("~/Content/plugins/jquery-ui/jquery-ui.min.css"));

            
            //style for plugins
            bundles.Add(new StyleBundle("~/bundle/morriscss").Include("~/Content/plugins/morris/morris.css"));
            bundles.Add(new StyleBundle("~/bundle/datepicker").Include("~/Content/plugins/bootstrap-datepicker/css/bootstrap-datepicker.min.css"));
            bundles.Add(new StyleBundle("~/bundle/toastrcss").Include("~/Content/plugins/toastr/toastr.min.css"));
            //bundles.Add(new StyleBundle("~/bundle/sweetalert").Include("~/Content/plugins/bootstrap-sweetalert/sweet-alert.css"));
            bundles.Add(new StyleBundle("~/bundle/switcherycss").Include("~/Content/plugins/switchery/switchery.min.css"));
            bundles.Add(new StyleBundle("~/bundle/jqueryfiler").Include(
                "~/Content/plugins/jquery.filer/css/themes/jquery.filer-dragdropbox-theme.css",
                "~/Content/plugins/jquery.filer/css/jquery.filer.css"));


            //style responsive datatable
            bundles.Add(new StyleBundle("~/bundle/responsivedatatablecss").Include(
                 "~/Content/plugins/datatables/jquery.dataTables.min.css",
                 "~/Content/plugins/datatables/buttons.bootstrap.min.css",
                 "~/Content/plugins/datatables/responsive.bootstrap.min.css",
                 "~/Content/plugins/datatables/dataTables.bootstrap.min.css"
                ));

            //JS Bundles
            bundles.Add(new ScriptBundle("~/bundle/jquery").Include("~/Content/assets/js/jquery.min.js"));
            //bundles.Add(new ScriptBundle("~/bundle/jqueryui").Include("~/Content/plugins/jquery-ui/jquery-ui.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/bootstrapjs").Include("~/Content/assets/js/bootstrap.min.js",
                "~/Content/plugins/bootstrap-tagsinput/js/bootstrap-tagsinput.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/detect").Include("~/Content/assets/js/detect.js"));
            bundles.Add(new ScriptBundle("~/bundle/fastclick").Include("~/Content/assets/js/fastclick.js"));
            bundles.Add(new ScriptBundle("~/bundle/blockUI").Include("~/Content/assets/js/jquery.blockUI.js"));
            bundles.Add(new ScriptBundle("~/bundle/waves").Include("~/Content/assets/js/waves.js"));
            bundles.Add(new ScriptBundle("~/bundle/slimscroll").Include("~/Content/assets/js/jquery.slimscroll.js"));
            bundles.Add(new ScriptBundle("~/bundle/scrollTo").Include("~/Content/assets/js/jquery.scrollTo.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/modernizr").Include("~/Content/assets/js/modernizr.min.js"));


            //App js 
            bundles.Add(new ScriptBundle("~/bundle/corejs").Include("~/Content/assets/js/jquery.core.js"));
            bundles.Add(new ScriptBundle("~/bundle/jqueryapp").Include("~/Content/assets/js/jquery.app.js"));

            //JS for Plugin
            bundles.Add(new ScriptBundle("~/bundle/switcheryjs").Include("~/Content/plugins/switchery/switchery.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/datepicker").Include("~/Content/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/jqueryfiler").Include("~/Content/plugins/jquery.filer/js/jquery.filer.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/toastrjs").Include("~/Content/plugins/toastr/toastr.min.js"));
            //bundles.Add(new ScriptBundle("~/bundle/sweetalert").Include("~/Content/plugins/bootstrap-sweetalert/sweet-alert.min.js"));
            bundles.Add(new ScriptBundle("~/bundle/momentjs").Include("~/Content/plugins/moment/moment.js"));

            //JS responsive datatable
            bundles.Add(new ScriptBundle("~/bundle/responsivedatatablejs").Include(
                "~/Content/plugins/datatables/jquery.dataTables.min.js",
                "~/Content/plugins/datatables/dataTables.bootstrap.js",
                "~/Content/plugins/datatables/dataTables.buttons.min.js",
                "~/Content/plugins/datatables/buttons.bootstrap.min.js",
                "~/Content/plugins/datatables/jszip.min.js",
                "~/Content/plugins/datatables/pdfmake.min.js",
                "~/Content/plugins/datatables/vfs_fonts.js",
                "~/Content/plugins/datatables/buttons.html5.min.js",
                "~/Content/plugins/datatables/buttons.print.min.js",
                "~/Content/plugins/datatables/dataTables.responsive.min.js",
                "~/Content/plugins/datatables/responsive.bootstrap.min.js",
                //init
                "~/Content/assets/pages/jquery.datatables.init.js"
                ));


            bundles.Add(new ScriptBundle("~/bundles/Script-calendar").Include(
                                 "~/Scripts/script-custom-calendar.js"));
            BundleTable.EnableOptimizations = true;
        }
    }
}
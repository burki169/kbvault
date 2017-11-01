using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.ViewEngines
{
    public class KbVaultViewEngine : RazorViewEngine
    {
        public KbVaultViewEngine():base()
        {            
            var viewLocations = new[] {
            "~/Views/{1}/{0}.cshtml",
            "~/Views/Themes/"+Theme+"/{1}/{0}.cshtml",
            "~/Views/Themes/"+Theme+"/Partials/{0}.cshtml",
            "~/Views/Shared/{0}.cshtml"                       
            // etc
             };

            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }

        private string Theme => ConfigurationManager.AppSettings["Theme"];                    
        /*
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
	    {	        
	        return base.CreatePartialView(controllerContext, partialPath.Replace("%THEME%", Theme));
	    }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {            
            return base.CreateView(controllerContext, viewPath.Replace("%THEME%", Theme), masterPath.Replace("%THEME%", Theme));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
	    {	        
	        return base.FileExists(controllerContext, virtualPath.Replace("%THEME%", Theme));
	    }
        */
    }
}
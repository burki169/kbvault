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
        public KbVaultViewEngine()
        {
            var viewLocations = new[]
            {
            "~/Views/{1}/{0}.cshtml",
            "~/Views/Themes/" + Theme + "/{1}/{0}.cshtml",
            "~/Views/Themes/" + Theme + "/Partials/{0}.cshtml",
            "~/Views/Shared/{0}.cshtml"
             };

            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }

        private string Theme => ConfigurationManager.AppSettings["Theme"];
    }
}
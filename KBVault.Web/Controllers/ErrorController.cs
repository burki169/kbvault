using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Controllers
{
    public class ErrorController : KbVaultAdminController
    {
        public ActionResult Index()
        {
            ViewBag.ErrorMessage = "...";
            var ex = GetGlobalException();
            if (ex != null)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        public ActionResult PublicError()
        {
            return View();
        }
    }
}

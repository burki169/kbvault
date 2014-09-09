using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Controllers
{
    public class AnalyticsController : KbVaultAdminController
    {
        //
        // GET: /Analytics/

        public ActionResult Index()
        {
            return View();
        }

    }
}

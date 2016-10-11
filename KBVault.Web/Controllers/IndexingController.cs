using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class IndexingController : KbVaultAdminController
    {
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
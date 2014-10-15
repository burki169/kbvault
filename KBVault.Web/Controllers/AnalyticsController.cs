using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;


namespace KBVault.Web.Controllers
{
    public class AnalyticsController : KbVaultAdminController
    {
        //
        // GET: /Analytics/

        public ActionResult Index()
        {
            using (var db = new KbVaultEntities())
            {

                Setting set = db.Settings.FirstOrDefault();
                if (set != null)
                {
                    
                }
            }
            return View();
        }

    }
}

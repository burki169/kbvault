using KBVault.Dal;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Controllers
{
    public class KbVaultPublicController : Controller
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected Setting Settings;
        
        public KbVaultPublicController()
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                Settings = db.Settings.FirstOrDefault( s => true);
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.CompanyName = Settings.CompanyName;
            ViewBag.TagLine = Settings.TagLine;
            ViewBag.DisqusShortName = Settings.DisqusShortName;
            ViewBag.ShareThisPublicKey = Settings.ShareThisPublicKey;
            ViewBag.ArticleDisplayCount = Settings.ArticleCountPerCategoryOnHomePage;

            base.OnActionExecuted(filterContext);
        }

    }
}

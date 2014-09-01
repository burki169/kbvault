using KBVault.Dal;
using KBVault.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            using(KbVaultEntities db = new KbVaultEntities())
            {
                DashboardViewModel model = new DashboardViewModel();
                model.TotalArticleCount = db.Articles.Count();
                model.MostLikedArticle = db.Articles.OrderByDescending(a => a.Likes).FirstOrDefault();
                model.MostViewedArticle = db.Articles.OrderByDescending(a => a.Views).FirstOrDefault();
                return View(model);
            }
        }

    }
}

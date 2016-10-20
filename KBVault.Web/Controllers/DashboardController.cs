using KBVault.Dal;
using KBVault.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal.Repository;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IArticleRepository ArticleRepository { get; set; }
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalArticleCount = ArticleRepository.GetTotalArticleCount(),
                MostLikedArticle = ArticleRepository.GetMostLikedArticle(),
                MostViewedArticle = ArticleRepository.GetMostViewedArticle()
            };
            return View(model);
            
        }

    }
}

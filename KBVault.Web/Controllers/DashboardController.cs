using System.Web.Mvc;
using KBVault.Dal.Repository;
using KBVault.Web.Models;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IArticleRepository ArticleRepository { get; set; }

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

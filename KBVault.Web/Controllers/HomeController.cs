using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Dal.Repository;
using KBVault.Web.Models;
using KBVault.Web.Models.Public;
using MvcPaging;
using Resources;

namespace KBVault.Web.Controllers
{
    public class HomeController : KbVaultPublicController
    {
        private const int ArticleCountPerPage = 20;

        public ITagRepository TagRepository { get; set; }
        public IArticleRepository ArticleRepository { get; set; }
        public ICategoryRepository CategoryRepository { get; set; }

        [HttpPost]
        public JsonResult Like(int articleId)
        {
            var result = new JsonOperationResponse();
            if (Request.IsAjaxRequest())
            {
                using (var db = new KbVaultContext())
                {
                    var article = db.Articles.FirstOrDefault(a => a.Id == articleId);
                    if (article == null)
                    {
                        result.ErrorMessage = ErrorMessages.ArticleNotFound;
                    }
                    else
                    {
                        article.Likes++;
                        db.SaveChanges();
                        result.Successful = true;
                        result.ErrorMessage = UIResources.ArticleLikeSuccess;
                    }
                }
            }

            return Json(result);
        }

        public ActionResult Tags(string id, int page = 1)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var tag = db.Tags.First(c => c.Name == id);
                    if (tag == null)
                    {
                        return View("TagNotFound");
                    }

                    ViewBag.Tag = tag;
                    IList<Article> articles = db.PublishedArticles().Where(a => a.ArticleTags.Any(t => t.Tag.Name == id)).OrderBy(a => a.Title).ToPagedList(page, ArticleCountPerPage);
                    return View(articles);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("PublicError", "Error");
            }
        }

        public ActionResult Categories(string id, int page = 1)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var cat = db.Categories.Include("ChildCategories").Include("ParentCategory").First(c => c.SefName == id);
                    if (cat == null)
                    {
                        return View("CategoryNotFound");
                    }

                    ViewBag.Category = cat;
                    IList<Article> articles = db.PublishedArticles().Where(a => a.Category.SefName == id).OrderBy(a => a.Title).ToPagedList(page, ArticleCountPerPage);
                    return View(articles);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("PublicError", "Error");
            }
        }

        public ActionResult Detail(string id)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var article = db.PublishedArticles().FirstOrDefault(a => a.SefName == id);
                    if (article != null)
                    {
                        article.Views++;
                        db.SaveChanges();
                        ViewBag.SimilarArticles = ArticleRepository.GetVisibleSimilarArticles((int)article.Id, DateTime.Today.Date);
                        return View(article);
                    }

                    return View("ArticleNotFound");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("PublicError", "Error");
            }
        }

        public ActionResult Index()
        {
            var settings = SettingsService.GetSettings();
            var model = new LandingPageViewModel();
            if (settings.ShowTotalArticleCountOnFrontPage)
            {
                model.TotalArticleCountMessage = string.Format(UIResources.PublicTotalArticleCountMessage, ArticleRepository.GetTotalArticleCount());
            }

            model.HotCategories = CategoryRepository.GetHotCategories().ToList();
            ViewBag.Title = settings.CompanyName;
            model.FirstLevelCategories = CategoryRepository.GetFirstLevelCategories().ToList();
            model.LatestArticles = ArticleRepository.GetLatestArticles(settings.ArticleCountPerCategoryOnHomePage);
            model.PopularArticles = ArticleRepository.GetPopularArticles(settings.ArticleCountPerCategoryOnHomePage);
            model.PopularTags = TagRepository.GetTagCloud().Select(tag => new TagCloudItem(tag)).ToList();
            return View(model);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Core.MVC.Authorization;
using KBVault.Web.Models.Public;
using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Dal.Repository;
using KBVault.Web.Models;
using MvcPaging;
using KBVault.Web.Helpers;
using Resources;

namespace KBVault.Web.Controllers
{
    public class HomeController : KbVaultPublicController
    {
        private int ArticleCountPerPage = 20;

        public ITagRepository TagRepository { get; set; }
        public IArticleRepository ArticleRepository { get; set; }
        

        [HttpPost]
        public JsonResult Like(int articleId)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            if (Request.IsAjaxRequest() )
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
                    Tag tag = db.Tags.First(c => c.Name == id);
                    if (tag == null)
                    {
                        return View("TagNotFound");
                    }
                    ViewBag.Tag = tag;
                    IList<Article> articles = db.PublishedArticles().Where(a => a.ArticleTags.Any(t => t.Tag.Name == id) ).OrderBy(a => a.Title).ToPagedList(page, ArticleCountPerPage);
                    return View(articles);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("PublicError", "Error");
            }
        }

        public ActionResult Categories(string id, int page=1)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    Category cat = db.Categories.Include("ChildCategories").Include("ParentCategory").First(c => c.SefName == id);
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
                    Article article = db.PublishedArticles().FirstOrDefault(a => a.SefName == id);                                  
                    if (article != null)
                    {
                        article.Views++;                        
                        db.SaveChanges();                        
                        ViewBag.SimilarArticles = ArticleRepository.GetVisibleSimilarArticles((int)article.Id, DateTime.Today.Date);
                        return View(article);
                    }
                    else
                    {
                        return View("ArticleNotFound");
                    }
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
            using (var db = new KbVaultContext())
            {                
                LandingPageViewModel model = new LandingPageViewModel();
                if (settings.ShowTotalArticleCountOnFrontPage)
                {
                    model.TotalArticleCountMessage = string.Format(UIResources.PublicTotalArticleCountMessage,db.PublishedArticles().Count());
                }
                model.HotCategories = db.Categories.Include("Articles").Where(c => c.IsHot).ToList();
                DateTime dateRangeToday = DateTime.Now.Date;
                ViewBag.Title = settings.CompanyName;
                model.FirstLevelCategories = db.Categories.Include("Articles").Where(c => c.Parent == null).OrderBy(c => c.Name).ToList();
                model.LatestArticles = db.PublishedArticles()                                       
                                        .OrderByDescending(a => a.Edited)
                                        .Take(settings.ArticleCountPerCategoryOnHomePage)
                                        .ToList();
                model.PopularArticles= db.PublishedArticles()                                            
                                            .OrderByDescending(a => a.Likes)
                                            .Take(settings.ArticleCountPerCategoryOnHomePage)
                                            .ToList();
                /* Build tag cloud */
                model.PopularTags = TagRepository.GetTopTags().OrderBy( c => Guid.NewGuid()).ToList();
                int ratioDiff = model.MaxTagRatio - model.MinTagRatio;
                int minRatio = model.MinTagRatio;
                foreach (var item in model.PopularTags)
                {
                    if (ratioDiff > 0)
                        item.FontSize = 80 + Convert.ToInt32(Math.Truncate((double)(item.Ratio - minRatio) * (100 / ratioDiff)));
                    else
                        item.FontSize = 80;
                }
                return View(model);
            }
            
        }

    }
}

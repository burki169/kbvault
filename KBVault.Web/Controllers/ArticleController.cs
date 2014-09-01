using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Helpers;
using KBVault.Web.Models;
using KBVault.Web.Resources;
using NLog;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class ArticleController : KbVaultAdminController
    {        

      
        [HttpPost]
        public JsonResult Remove(int id)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    //Remove article and attachments
                    Article article = db.Articles.First(a => a.Id == id);
                    if (article == null)
                        throw new Exception(ErrorMessages.ArticleNotFound);
                    foreach (Attachment attach in article.Attachments)
                    {
                        KbVaultAttachmentHelper.RemoveAttachment(attach.Hash);                        
                    }
                    db.Articles.Remove(article);
                    db.SaveChanges();
                    result.Data = id;
                    result.Successful = true;
                    return Json(result);
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.Successful = false;
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }
        
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "Category.Name,Category.SefName")]ArticleViewModel model)
        {
            try
            {                
                ModelState.Remove("Category.Name");
                ModelState.Remove("Category.SefName");
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        Article article = db.Articles.FirstOrDefault(a => a.Id == model.Id);
                        article.CategoryId = model.Category.Id;
                        article.IsDraft = model.IsDraft ? 1 : 0;
                        article.PublishEndDate = model.PublishEndDate;
                        article.PublishStartDate = model.PublishStartDate;                        
                        article.Edited = DateTime.Now;
                        article.Title = model.Title;
                        article.Content = model.Content;
                        article.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                        article.SefName = model.SefName;
                        if (!String.IsNullOrEmpty(model.Tags))
                            db.AssignTagsToArticle(article.Id, model.Tags);
                        db.SaveChanges();
                        if (article.IsDraft == 0)
                            KbVaultLuceneHelper.AddArticleToIndex(article);
                        else
                            KbVaultLuceneHelper.RemoveArticleFromIndex(article);
                    }

                    ShowOperationMessage(UIResources.ArticleCreatePageEditSuccessMessage);                    
                }
                return View("Create",model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ModelState.AddModelError("Exception", ex.Message);
                return View("Create", model);
                
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    ArticleViewModel model = new ArticleViewModel();
                    Article article = db.Articles.First(a => a.Id == id);
                    model.Author = db.KbUsers.First(u => u.Id == article.Author);
                    model.Category = new CategoryViewModel(article.Category);
                    model.Content = article.Content;
                    model.Created = article.Created.HasValue?(DateTime) article.Created:DateTime.Now;
                    model.Edited = article.Edited.HasValue?(DateTime)article.Edited:DateTime.Now;
                    model.Id = article.Id;
                    model.IsDraft = article.IsDraft==1?true:false;
                    model.Likes = article.Likes;
                    model.PublishEndDate = article.PublishEndDate.HasValue ? (DateTime)article.PublishEndDate : DateTime.Now;
                    model.PublishStartDate = article.PublishStartDate.HasValue ? (DateTime)article.PublishStartDate : DateTime.Now.AddYears(5);
                    model.Title = article.Title;
                    model.Tags = String.Join(",", article.ArticleTags.Select(at => at.Tag.Name).ToArray());
                    model.Attachments = article.Attachments.Select(t => new AttachmentViewModel(t)).ToList();
                    model.SefName = article.SefName;
                    return View("Create",model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult Create(ArticleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {                        
                        Article article = new Article();
                        article.CategoryId = model.Category.Id;
                        article.IsDraft = model.IsDraft?1:0;
                        article.PublishEndDate = model.PublishEndDate;
                        article.PublishStartDate = model.PublishStartDate;
                        article.Created = DateTime.Now;
                        article.Edited = DateTime.Now;
                        article.Title = model.Title;
                        article.Content = model.Content;
                        article.SefName = model.SefName;
                        article.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                        db.Articles.Add(article);
                        db.SaveChanges();
                        if( !String.IsNullOrEmpty(model.Tags) )
                            db.AssignTagsToArticle(article.Id, model.Tags);
                        db.SaveChanges();
                        if( article.IsDraft == 0 )
                            KbVaultLuceneHelper.AddArticleToIndex(article);
                        ShowOperationMessage(UIResources.ArticleCreatePageCreateSuccessMessage);
                        return RedirectToAction("Edit", "Article", new { id = article.Id});
                    }
                }                
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ModelState.AddModelError("Exception",ex.Message);
                return View(model);
            }
        }
        public ActionResult Create(int id= -1)
        {
            try
            {
                using( KbVaultEntities db = new KbVaultEntities())
                {
                    ArticleViewModel model = new ArticleViewModel();
                    model.Category = new CategoryViewModel();
                    if (id > 1)
                    {
                        Category cat = db.Categories.First(c => c.Id == id);
                        model.Category.Id = cat.Id;
                        model.Category.Name = cat.Name;
                        model.Category.SefName = cat.SefName;
                    }
                    else
                    {
                        model.Category.Name = "-";
                        model.Category.SefName= "-";
                    }
                    model.PublishStartDate = DateTime.Now.Date;
                    model.PublishEndDate = DateTime.Now.AddYears(5).Date;
                    
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

    }
}

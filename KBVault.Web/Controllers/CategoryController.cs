using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Models;
using NLog;
using MvcPaging;
using Resources;
using KBVault.Web.Helpers;
using ICategoryFactory = KBVault.Web.Business.Categories.ICategoryFactory;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class CategoryController : KbVaultAdminController
    {        
        public ICategoryRepository CategoryRepository { get; set; }
        public IArticleRepository ArticleRepository { get; set; }
        public ICategoryFactory CategoryFactory { get; set; }

        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var parentId = model.ParentId > 0 ? model.ParentId : (int?) null;
                    var category = CategoryFactory.CreateCategory(model.Name, model.IsHot, model.SefName,KBVaultHelperFunctions.UserAsKbUser(User).Id, parentId);
                    var catId = CategoryRepository.Add(category);
                    ShowOperationMessage(@UIResources.CategoryPageCreateSuccessMessage);
                    return RedirectToAction("List", new { id = catId, page = 1 });                                        
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
        }
        

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public JsonResult Remove(int id)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    Category cat = db.Categories.First(c => c.Id == id);
                    if (cat != null)
                    {
                        if (cat.Articles.Count() == 0)
                        {
                            cat.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                            db.Categories.Remove(cat);
                            db.SaveChanges();
                            result.Successful = true;                            
                            result.ErrorMessage = String.Format(ErrorMessages.CategoryRemovedSuccessfully, cat.Name);
                            
                            UrlHelper url = new UrlHelper(Request.RequestContext);
                            cat = db.Categories.First();
                            if (cat == null)
                                result.Data = url.Action("Index", "Dashboard");
                            else
                                result.Data = url.Action("List", "Category", new { id = cat.Id, page = 1 });
                        }
                        else
                        {
                            result.Successful = false;
                            result.ErrorMessage = ErrorMessages.CategoryIsNotEmpty;
                        }
                    }
                    else
                    {
                        result.Successful = false;
                        result.ErrorMessage = @ErrorMessages.CategoryNotFound;
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.Successful = false;
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int id = -1)
        {
            try
            {
                return View(CategoryFactory.CreateCategoryViewModel(CategoryRepository.Get(id)));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var parentId = model.ParentId > 0 ? model.ParentId : (int?) null;
                        var author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                        var category = CategoryFactory.CreateCategory(model.Name, model.IsHot, model.SefName,author, parentId);
                        category.Id = model.Id;
                        CategoryRepository.Update(category);
                        ShowOperationMessage(UIResources.CategoryPageEditSuccessMessage);
                        return RedirectToAction("List", new {id = model.Id, page = 1});
                    }
                    catch (ArgumentNullException)
                    {
                        ModelState.AddModelError("Category Not Found", ErrorMessages.CategoryNotFound);
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
        
        //List articles in category
        public ActionResult List(int id,int page)
        {
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    Category cat = db.Categories.First(a => a.Id == id);
                    ViewBag.CategoryName = cat.Name;
                    ViewBag.CategoryId = cat.Id;
                    IList<Article> articles = db.Articles.Where(a => a.CategoryId == id).OrderBy(a => a.Title).ToPagedList(page, 20);
                    return View(articles);
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

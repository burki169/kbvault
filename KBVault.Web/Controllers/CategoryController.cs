using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Models;
using NLog;
using MvcPaging;
using KBVault.Web.Resources;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class CategoryController : KbVaultAdminController
    {        

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create(CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        Category cat = new Category();
                        cat.Name = model.Name;
                        if( model.ParentId > 0 )
                            cat.Parent = model.ParentId;
                        cat.IsHot = model.IsHot;
                        cat.SefName = model.SefName;
                        db.Categories.Add(cat);                        
                        db.SaveChanges();
                        ShowOperationMessage(@UIResources.CategoryPageCreateSuccessMessage);
                        return RedirectToAction("List", new { id = cat.Id, page = 1 });
                    }
                    
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
        }
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            return View();
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

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        Category cat = db.Categories.First(c => c.Id == model.Id);
                        if (cat != null)
                        {
                            cat.Name = model.Name;
                            cat.IsHot = model.IsHot;
                            cat.SefName = model.SefName;
                            if (model.ParentId > 0)
                                cat.Parent = model.ParentId;
                            else
                                cat.Parent = (int?)null;
                            db.SaveChanges();
                            ShowOperationMessage(UIResources.CategoryPageEditSuccessMessage);
                            return RedirectToAction("List", new { id= model.Id, page=1});
                        }
                        
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
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int id = -1)
        {
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    CategoryViewModel categoryModel = new CategoryViewModel();
                    Category category = db.Categories.First(ca => ca.Id == id);
                    if (category == null)
                    {
                        throw new ArgumentNullException("Category not found");
                    }
                    categoryModel.Id = category.Id;
                    categoryModel.IsHot = category.IsHot;
                    categoryModel.ParentId = category.Parent.HasValue ? (int)category.Parent : -1;
                    categoryModel.Name = category.Name;
                    categoryModel.SefName = category.SefName;
                    return View(categoryModel);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
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

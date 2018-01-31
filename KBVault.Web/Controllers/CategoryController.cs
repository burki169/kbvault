using System;
using System.Web.Mvc;
using KBVault.Dal.Entities;
using KBVault.Dal.Repository;
using KBVault.Web.Helpers;
using KBVault.Web.Models;
using MvcPaging;
using Resources;
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
                    var parentId = model.ParentId > 0 ? model.ParentId : (int?)null;
                    var category = CategoryFactory.CreateCategory(model.Name, model.IsHot, model.SefName, model.Icon, KBVaultHelperFunctions.UserAsKbUser(User).Id, parentId);
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
            var result = new JsonOperationResponse();
            try
            {
                if (CategoryRepository.Get(id) != null)
                {
                    if (!CategoryRepository.HasArticleInCategory(id))
                    {
                        var cat = new Category
                        {
                            Author = KBVaultHelperFunctions.UserAsKbUser(User).Id,
                            Id = id
                        };
                        if (CategoryRepository.Remove(cat))
                        {
                            result.Successful = true;
                            result.ErrorMessage = string.Format(ErrorMessages.CategoryRemovedSuccessfully, cat.Name);

                            var url = new UrlHelper(Request.RequestContext);
                            cat = CategoryRepository.GetFirstCategory();
                            result.Data = cat == null
                                ? url.Action("Index", "Dashboard")
                                : url.Action("List", "Category", new { id = cat.Id, page = 1 });
                        }
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
                    result.ErrorMessage = ErrorMessages.CategoryNotFound;
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
                        var parentId = model.ParentId > 0 ? model.ParentId : (int?)null;
                        var author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                        var category = CategoryFactory.CreateCategory(model.Name, model.IsHot, model.SefName, model.Icon, author, parentId);
                        category.Id = model.Id;
                        CategoryRepository.Update(category);
                        ShowOperationMessage(UIResources.CategoryPageEditSuccessMessage);
                        return RedirectToAction("List", new { id = model.Id, page = 1 });
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
                ModelState.AddModelError("Exception", ex.Message);
                return View(model);
            }
        }

        public ActionResult List(int id, int page)
        {
            try
            {
                var cat = CategoryRepository.Get(id);
                if (cat != null)
                {
                    var model = new CategoryListViewModel
                    {
                        CategoryName = cat.Name,
                        CategoryId = cat.Id,
                        Icon = cat.Icon,
                        Articles = CategoryRepository.GetArticles(id).ToPagedList(page, 20)
                    };
                    return View(model);
                }

                ShowOperationMessage(ErrorMessages.CategoryNotFound);
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}

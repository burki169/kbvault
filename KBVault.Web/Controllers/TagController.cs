using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using NLog;
using MvcPaging;
using KBVault.Web.Models;
using KBVault.Web.Resources;

namespace KBVault.Web.Controllers
{
    [Authorize(Roles="Admin,Manager")]
    public class TagController : KbVaultAdminController
    {
        private Logger Log = LogManager.GetCurrentClassLogger();
        private int PageSize = 45;

        [HttpPost]
        public JsonResult Edit(string name, string pk, string value)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    long tagId = Convert.ToInt64(pk);
                    Tag tag = db.Tags.First(t => t.Id == tagId);
                    if (tag != null)
                    {
                        tag.Name = value;
                        db.SaveChanges();
                        result.Successful = true;
                        return Json(result);
                    }
                }
                return null;
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
        public JsonResult Remove(int id = -1)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            result.Successful = false;
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    
                    Tag tag = db.Tags.First(t => t.Id == id);
                    if (tag != null)
                    {
                        db.Tags.Remove(tag);
                        db.RemoveTagFromArticles(id);
                        db.SaveChanges();
                        result.Successful = true;
                        result.ErrorMessage = UIResources.TagListRemoveSuccessful;
                    }
                    else
                    {
                        result.ErrorMessage = ErrorMessages.TagNotFound;
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);                
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

        public ActionResult List(int page = 1)
        {
            try
            {
                if (page < 1)
                    page = 1;
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    IList<Tag> Tags = db.Tags.OrderBy(t => t.Name).ToPagedList(page, PageSize);
                    return View(Tags);
                }                
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return RedirectToAction("Index", "Error");
            }
            
        }

        [Authorize(Roles="Admin,Manager,Editor")]
        public JsonResult Suggest(string term)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            try
            {                
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    var suggestions = db.Tags.Where(t => t.Name.Contains(term)).Select(t => t.Name).Take(20).ToList<string>();
                    result.Successful = true;
                    result.Data = suggestions.ToArray();
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
    }
}

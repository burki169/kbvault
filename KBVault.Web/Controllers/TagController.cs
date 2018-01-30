using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Dal.Repository;
using NLog;
using MvcPaging;
using KBVault.Web.Models;

using Resources;
using KBVault.Web.Helpers;

namespace KBVault.Web.Controllers
{
    [Authorize(Roles="Admin,Manager")]
    public class TagController : KbVaultAdminController
    {

        private int PageSize = 45;

        public ITagRepository TagRepository { get; set; }

        [HttpPost]
        public JsonResult Edit(string name, string pk, string value)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            try
            {
                using (var db = new KbVaultContext())
                {
                    long tagId = Convert.ToInt64(pk);
                    Tag tag = db.Tags.First(t => t.Id == tagId);
                    if (tag != null)
                    {
                        tag.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
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
            var result = new JsonOperationResponse
            {
                Successful = false
            };
            try
            {
                using (var db = new KbVaultContext())
                {
                    var tag = db.Tags.First(t => t.Id == id);
                    if (tag != null)
                    {
                        tag.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                        db.Tags.Remove(tag);
                        TagRepository.RemoveTagFromArticles(id);
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
                {
                    page = 1;
                }

                using (var db = new KbVaultContext())
                {
                    var Tags = db.Tags.OrderBy(t => t.Name).ToPagedList(page, PageSize);
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
            var result = new JsonOperationResponse();
            try
            {
                using (var db = new KbVaultContext())
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

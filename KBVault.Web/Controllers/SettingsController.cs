using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Web.Models;
using KBVault.Dal;
using KBVault.Web.Resources;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class SettingsController : KbVaultAdminController
    {
        //private Logger Log = LogManager.GetCurrentClassLogger();
        //
        // GET: /Settings/
        [HttpPost]
        public ActionResult Index(SettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        Setting set = db.Settings.FirstOrDefault();
                        if (set != null)
                        {
                            db.Settings.Remove(set);
                        }
                        set = new Setting();   
                        set.CompanyName = model.CompanyName;
                        set.ArticleCountPerCategoryOnHomePage = model.ArticleCountPerCategoryOnHomePage;
                        set.DisqusShortName = model.DisqusShortName;
                        set.JumbotronText = model.JumbotronText;
                        set.ShareThisPublicKey = model.ShareThisPublicKey;
                        set.TagLine = model.TagLine;
                        set.IndexFileExtensions = model.IndexFileExtensions;
                        db.Settings.Add(set);
                        db.SaveChanges();
                        ShowOperationMessage(UIResources.SettingsPageSaveSuccessfull);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowErrorMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Index()
        {
            try
            {
                using(KbVaultEntities db = new KbVaultEntities())
                {
                    ViewBag.UpdateSuccessfull = false;
                    Setting set = db.Settings.FirstOrDefault();
                    SettingsViewModel model = new SettingsViewModel(set);                    
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowErrorMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

    }
}

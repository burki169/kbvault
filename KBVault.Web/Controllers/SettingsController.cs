using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Web.Models;
using KBVault.Dal;
using KBVault.Dal.Entities;
using Resources;
using KBVault.Web.Helpers;
using System.Reflection;

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
                    using (var db = new KbVaultContext())
                    {
                        Settings set = db.Settings.FirstOrDefault();
                        if (set != null)
                        {
                            db.Settings.Remove(set);
                        }
                        set = new Settings();   
                        set.CompanyName = model.CompanyName;
                        set.ArticleCountPerCategoryOnHomePage = model.ArticleCountPerCategoryOnHomePage;
                        set.DisqusShortName = model.DisqusShortName;
                        set.JumbotronText = model.JumbotronText;
                        set.ShareThisPublicKey = model.ShareThisPublicKey;
                        set.TagLine = model.TagLine;
                        set.IndexFileExtensions = model.IndexFileExtensions;
                        set.ArticlePrefix = model.ArticlePrefix;
                        set.AnalyticsAccount = model.AnalyticsAccount;
                        set.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;                        
                        set.BackupPath = model.BackupPath;                        
                        set.ShowTotalArticleCountOnFrontPage = model.ShowTotalArticleCountOnFrontPage;
                        if (!string.IsNullOrEmpty(set.BackupPath))
                        {
                            if (!set.BackupPath.EndsWith("\\") && !set.BackupPath.StartsWith("~"))
                                set.BackupPath += "\\";
                            if (!set.BackupPath.EndsWith("/") && set.BackupPath.StartsWith("~"))
                                set.BackupPath += "/";
                        }
                        ConfigurationManager.AppSettings["Theme"] = model.SelectedTheme;
                        db.Settings.Add(set);
                        db.SaveChanges();
                        ShowOperationMessage(UIResources.SettingsPageSaveSuccessfull);
                    }
                }
                model.Themes.AddRange(Directory.EnumerateDirectories(Server.MapPath("~/Views/Themes")).Select(e => Path.GetFileName(e)).ToList());
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
                using(var db = new KbVaultContext())
                {
                    ViewBag.UpdateSuccessfull = false;
                    Settings set = db.Settings.FirstOrDefault();
                    SettingsViewModel model = new SettingsViewModel(set);
                    model.SelectedTheme = ConfigurationManager.AppSettings["Theme"];
                    var a = typeof(SettingsController).Assembly;
                    model.ApplicationVersion = a.GetName().Version.Major + "." + a.GetName().Version.Minor;                    
                    model.Themes.AddRange(Directory.EnumerateDirectories(Server.MapPath("~/Views/Home/Themes")).Select(e => Path.GetFileName(e)).ToList());                    
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

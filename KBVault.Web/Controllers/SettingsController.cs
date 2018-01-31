using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using KBVault.Dal.Repository;
using KBVault.Web.Business.ApplicationSettings;
using KBVault.Web.Models;
using Resources;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class SettingsController : KbVaultAdminController
    {
        public ISettingsFactory SettingsFactory { get; set; }
        public ISettingsService SettingsService { get; set; }
        public ISettingsRepository SettingsRepository { get; set; }

        [HttpPost]
        public ActionResult Index(SettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var set = SettingsFactory.CreateModel(model);
                    if (set != null)
                    {
                        SettingsRepository.Save(set);
                        ConfigurationManager.AppSettings["Theme"] = model.SelectedTheme;
                        SettingsService.ReloadSettings();
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
            ViewBag.UpdateSuccessfull = false;
            var model = SettingsFactory.CreateViewModel(SettingsService.GetSettings());
            return View(model);
        }
    }
}

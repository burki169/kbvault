using KBVault.Dal;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal.Entities;
using KBVault.Web.Business.ApplicationSettings;

namespace KBVault.Web.Controllers
{
    public class KbVaultPublicController : Controller
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();        
        public ISettingsService SettingsService { get; set; }
                

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var Settings = SettingsService.GetSettings();
            if (Settings != null)
            {
                ViewBag.CompanyName = Settings.CompanyName;
                ViewBag.JumbotronText = Settings.JumbotronText;
                ViewBag.TagLine = Settings.TagLine;
                ViewBag.DisqusShortName = Settings.DisqusShortName;
                ViewBag.ShareThisPublicKey = Settings.ShareThisPublicKey;
                ViewBag.ArticleDisplayCount = Settings.ArticleCountPerCategoryOnHomePage;
                ViewBag.ArticlePrefix = Settings.ArticlePrefix;
                ViewBag.AnalyticsAccount = Settings.AnalyticsAccount;
                ViewBag.Theme = ConfigurationManager.AppSettings["Theme"];
                ViewBag.ShowTotalArticleCountOnFrontPage = Settings.ShowTotalArticleCountOnFrontPage;
            }
            else
            {
                ViewBag.CompanyName = "Login as admin and set your configuration parameters";
            }

            base.OnActionExecuted(filterContext);
        }

    }
}

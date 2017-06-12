using KBVault.Dal;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal.Entities;

namespace KBVault.Web.Controllers
{
    public class KbVaultPublicController : Controller
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();
        protected Settings Settings;
        
        public KbVaultPublicController()
        {
            using (var db = new KbVaultContext())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                    Settings = db.Settings.FirstOrDefault( s => true);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
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
                ViewBag.Theme = Settings.SelectedTheme;
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

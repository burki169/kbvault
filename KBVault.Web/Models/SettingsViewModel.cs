
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KBVault.Dal;
using Resources;

namespace KBVault.Web.Models
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
        }

        public SettingsViewModel(Setting set)
        {
            if (set != null)
            {
                this.CompanyName = set.CompanyName;
                this.ArticleCountPerCategoryOnHomePage = set.ArticleCountPerCategoryOnHomePage;
                this.DisqusShortName = set.DisqusShortName;
                this.JumbotronText = set.JumbotronText;
                this.ShareThisPublicKey = set.ShareThisPublicKey;
                this.TagLine = set.TagLine;
                this.IndexFileExtensions = set.IndexFileExtensions;
                this.ArticlePrefix = set.ArticlePrefix;
                this.AnalyticsAccount = set.AnalyticsAccount;
                this.BackupPath = set.BackupPath;
                
            }
        }

        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "SettingsCompanyNameRequiredMessage")]
        public string CompanyName { get; set; }
        public string TagLine { get; set; }
        public string JumbotronText { get; set; }
        [RegularExpression(@"^\d+$",ErrorMessageResourceType=typeof(UIResources),ErrorMessageResourceName="SettingsPageArticleCountNumericErrorMessage")]        
        public short ArticleCountPerCategoryOnHomePage { get; set; }
        public string ShareThisPublicKey { get; set; }
        public string DisqusShortName { get; set; }
        public string IndexFileExtensions { get; set; }
        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "SettingsArticlePrefixRequiredMessage")]
        public string ArticlePrefix { get; set; }
        public string AnalyticsAccount { get; set; }
        public string BackupPath { get; set; }
    }
}
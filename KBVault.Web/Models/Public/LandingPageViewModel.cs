using KBVault.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KBVault.Dal.Entities;
using KBVault.Dal.Types;

namespace KBVault.Web.Models.Public
{
    public class LandingPageViewModel
    {
        public List<Category> HotCategories { get; set; }
        public List<Category> FirstLevelCategories { get; set; }
        public List<Article> LatestArticles { get; set; }
        public List<Article> PopularArticles { get; set; }
        public List<TagCloudItem> PopularTags { get; set; }
        public string TotalArticleCountMessage { get; set; }
        
    }
}
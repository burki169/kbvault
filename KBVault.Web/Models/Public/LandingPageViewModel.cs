using System.Collections.Generic;
using KBVault.Dal.Entities;

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
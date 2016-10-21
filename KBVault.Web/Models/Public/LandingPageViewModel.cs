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
        public List<TopTagItem> PopularTags { get; set; }
        public string TotalArticleCountMessage { get; set; }
        public int MaxTagRatio
        {
            get
            {                
                if (PopularTags.Max(t => t.Ratio).HasValue)
                    return Convert.ToInt32(PopularTags.Max(t => t.Ratio).Value);
                else
                    return -1;
            }
        }

        public int MinTagRatio
        {
            get
            {                
                if (PopularTags.Min(t => t.Ratio).HasValue)
                    return Convert.ToInt32(PopularTags.Min(t => t.Ratio).Value);
                else
                    return -1;
            }
        }
    }
}
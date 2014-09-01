using KBVault.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KBVault.Web.Models.Public
{
    public class LandingPageViewModel
    {
        public List<Category> HotCategories { get; set; }
        public List<Category> FirstLevelCategories { get; set; }
        public List<Article> LatestArticles { get; set; }
        public List<Article> PopularArticles { get; set; }
        public List<TopTagItem> PopularTags { get; set; }
        public int MaxTagRatio
        {
            get
            {
                int? maxAmount = PopularTags.Max(t => t.Ratio).Value;
                if (maxAmount.HasValue)
                    return Convert.ToInt32(maxAmount);
                else
                    return -1;
            }
        }

        public int MinTagRatio
        {
            get
            {
                int? minAmount = PopularTags.Min(t => t.Ratio).Value;
                if (minAmount.HasValue)
                    return Convert.ToInt32(minAmount);
                else
                    return -1;
            }
        }
    }
}
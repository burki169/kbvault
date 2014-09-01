using KBVault.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KBVault.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalArticleCount { get; set; }
        public Article MostLikedArticle { get; set; }
        public Article MostViewedArticle { get; set; }        
    }
}
using System;
using System.Linq;
using KBVault.Dal.Entities;

namespace KBVault.Dal
{
    public static class KbVaultDalExtensions
    {
        public static IQueryable<Article> PublishedArticles(this KbVaultContext db)
        {
            DateTime today = DateTime.Now.Date;
            return db.Articles
                    .Include("Category")
                    .Include("ArticleTags.Tag")
                    .Include("Attachments")
                    .Where(a => a.PublishStartDate <= today &&
                           a.PublishEndDate >= today &&
                           a.IsDraft == 0);
        }
    }
}

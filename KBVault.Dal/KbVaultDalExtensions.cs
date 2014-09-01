using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBVault.Dal
{
    public static class KbVaultDalExtensions
    {
        public static IQueryable<Article> PublishedArticles(this KbVaultEntities db)
        {
            try
            {
                DateTime today = DateTime.Now.Date;
                return db.Articles
                        .Include("Category")
                        .Include("ArticleTags.Tag")
                        .Include("Attachments")
                        .Where( a => a.PublishStartDate <= today &&
                                a.PublishEndDate >= today &&
                                a.IsDraft == 0
                        );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

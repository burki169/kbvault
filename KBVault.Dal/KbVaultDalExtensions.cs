using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using KBVault.Dal.Entities;

namespace KBVault.Dal
{    
        
    public static class KbVaultDalExtensions
    {
        public static IQueryable<Article> PublishedArticles(this KbVaultContext db)
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

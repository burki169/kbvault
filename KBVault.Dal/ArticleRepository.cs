using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace KBVault.Dal
{
    public class ArticleRepository : IArticleRepository
    {
        public Article Get(long id)
        {
            using (var db = new KbVaultEntities())
            {
                var model = db.Articles
                    .Include(c => c.Category)
                    .Include(t => t.ArticleTags.Select(a => a.Tag))                    
                    .Include(a => a.Attachments)                    
                    .FirstOrDefault(a => a.Id == id);
                return model;
            }
        }

        public long Add(Article article, string tags)
        {
            using (var db = new KbVaultEntities())
            {
                db.Articles.Add(article);
                db.SaveChanges();
                if (!String.IsNullOrEmpty(tags))
                    db.AssignTagsToArticle(article.Id, tags);
                db.SaveChanges();
                return article.Id;
            }
        }

        public void Update(Article article, string tags)
        {
            using (var db = new KbVaultEntities())
            {
                db.Articles.AddOrUpdate(article);
                db.SaveChanges();
                if (!String.IsNullOrEmpty(tags))
                    db.AssignTagsToArticle(article.Id, tags);
                db.SaveChanges();                
            }
        }
    }
}
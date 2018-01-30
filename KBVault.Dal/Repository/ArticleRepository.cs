using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using KBVault.Dal.Entities;
using KBVault.Dal.Types;

namespace KBVault.Dal.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        public Article Get(long id)
        {
            using (var db = new KbVaultContext())
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
            using (var db = new KbVaultContext())
            {
                db.Articles.Add(article);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(tags))
                {
                    AssignTagsToArticle(article.Id, tags);
                }

                db.SaveChanges();
                return article.Id;
            }
        }

        public void Update(Article article, string tags)
        {
            using (var db = new KbVaultContext())
            {
                db.Articles.AddOrUpdate(article);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(tags))
                {
                    AssignTagsToArticle(article.Id, tags);
                }

                db.SaveChanges();
            }
        }

        public IList<SimilarArticle> GetVisibleSimilarArticles(int articleId, DateTime date)
        {
            using (var db = new KbVaultContext())
            {
                var articleIdParam = new SqlParameter("ArticleId", articleId);
                return db.Database.SqlQuery<SimilarArticle>("exec GetSimilarArticles @ArticleId", articleIdParam).Where(a => a.PublishStartDate <= date && a.PublishEndDate >= date && a.IsDraft == 0).ToList();
            }
        }

        public void AssignTagsToArticle(long articleId, string tags)
        {
            using (var db = new KbVaultContext())
            {
                var articleIdParameter = new SqlParameter("ArticleId", articleId);
                var tagsParameter = new SqlParameter("Tags", tags);
                db.Database.ExecuteSqlCommand("exec AssignTagsToArticle @ArticleId, @Tags", articleIdParameter, tagsParameter);
            }
        }

        public int GetTotalArticleCount()
        {
            using (var db = new KbVaultContext())
            {
                return db.Articles.Count();
            }
        }

        public int GetTotalPublishedArticleCount()
        {
            using (var db = new KbVaultContext())
            {
                return db.PublishedArticles().Count();
            }
        }

        public Article GetMostLikedArticle()
        {
            using (var db = new KbVaultContext())
            {
                return db.Articles.OrderByDescending(a => a.Likes).FirstOrDefault();
            }
        }

        public Article GetMostViewedArticle()
        {
            using (var db = new KbVaultContext())
            {
                return db.Articles.OrderByDescending(a => a.Views).FirstOrDefault();
            }
        }

        public List<Article> GetLatestArticles(int maxItemCount)
        {
            using (var db = new KbVaultContext())
            {
                return db.PublishedArticles()
                    .OrderByDescending(a => a.Edited)
                    .Take(maxItemCount)
                    .ToList();
            }
        }

        public List<Article> GetPopularArticles(int maxItemCount)
        {
            using (var db = new KbVaultContext())
            {
                return db.PublishedArticles()
                    .OrderByDescending(a => a.Likes)
                    .Take(maxItemCount)
                    .ToList();
            }
        }
    }
}
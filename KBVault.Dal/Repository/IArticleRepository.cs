using System;
using System.Collections.Generic;
using KBVault.Dal.Entities;
using KBVault.Dal.Types;

namespace KBVault.Dal.Repository
{
    public interface IArticleRepository
    {
        Article Get(long id);
        long Add(Article article, string tags);
        void Update(Article article, string tags);
        IList<SimilarArticle> GetVisibleSimilarArticles(int articleId, DateTime date);
        void AssignTagsToArticle(long articleId, string tags);
        int GetTotalArticleCount();
        int GetTotalPublishedArticleCount();
        Article GetMostLikedArticle();
        Article GetMostViewedArticle();
        List<Article> GetLatestArticles(int maxItemCount);
        List<Article> GetPopularArticles(int maxItemCount);
    }
}
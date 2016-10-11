using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KBVault.Dal;
using KBVault.Web.Helpers;
using Microsoft.AspNet.SignalR;

namespace KBVault.Web.Hubs
{
    public class IndexerHub : Hub
    {
        public ICategoryRepository CategoryRepository { get; set; }
        public IArticleRepository ArticleRepository { get; set; }

        public void RebuildIndexes()
        {
            var categories = CategoryRepository.GetAllCategories();
            int totalCategories = categories.Count();
            int indexingCategory = 0;
            foreach (var cat in categories)
            {
                Clients.All.updateProgress(indexingCategory, totalCategories, cat.Name, "-");
                var articles = CategoryRepository.GetArticles(cat.Id);
                foreach (var article in articles)
                {
                    Clients.All.updateProgress(indexingCategory, totalCategories, cat.Name, article.Title);
                    foreach (var attachment in article.Attachments)
                    {
                        KbVaultLuceneHelper.RemoveAttachmentFromIndex(attachment);
                        KbVaultLuceneHelper.AddAttachmentToIndex(attachment);
                    }
                    KbVaultLuceneHelper.RemoveArticleFromIndex(article);
                    KbVaultLuceneHelper.AddArticleToIndex(article);                    
                }                
            }
        }
    }
}
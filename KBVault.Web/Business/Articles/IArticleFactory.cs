using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Web.Models;

namespace KBVault.Web.Business.Articles
{
    public interface IArticleFactory
    {
        ArticleViewModel CreateArticleViewModel(Article article);
        Article CreateArticleFromViewModel(ArticleViewModel articleViewModel, long userId);
        ArticleViewModel CreateArticleViewModelWithDefValues(Category cat);
    }
}
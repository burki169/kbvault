using KBVault.Dal;
using KBVault.Web.Models;

namespace KBVault.Web.Business.Categories
{
    public interface IArticleFactory
    {
        ArticleViewModel CreateArticleViewModel(Article article);
        Article CreateArticleFromViewModel(ArticleViewModel articleViewModel, long userId);
        ArticleViewModel CreateArticleViewModelWithDefValues(Category cat);
    }
}
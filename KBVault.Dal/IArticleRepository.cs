using System.Collections.Generic;

namespace KBVault.Dal
{
    public interface IArticleRepository
    {
        IList<Article> GetArticlesInCategory(int categoryId, int page);
    }
}
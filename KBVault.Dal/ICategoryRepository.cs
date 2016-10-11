using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KBVault.Dal
{
    public interface ICategoryRepository
    {
        int Add(Category category);
        void Update(Category category);
        Category Get(int categoryId);        
        bool Remove(Category category);

        Category GetFirstCategory();
        IList<Category> GetAllCategories();
        bool HasArticleInCategory(int categoryId);
        IList<Article> GetArticles(int categoryId);
    }
}
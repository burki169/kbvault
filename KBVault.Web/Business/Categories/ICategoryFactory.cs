using KBVault.Dal;
using KBVault.Web.Models;

namespace KBVault.Web.Business.Categories
{
    public interface ICategoryFactory
    {
        Category CreateCategory(string name, bool isHot, string sefName, long author, int? parent);
        CategoryViewModel CreateCategoryViewModel(Category cat);
    }
}
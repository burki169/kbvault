using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Web.Models;

namespace KBVault.Web.Business.Categories
{
    public class CategoryFactory : ICategoryFactory
    {
        public Category CreateCategory(string name, bool isHot, string sefName, string icon, long author, int? parent)
        {
            return new Category
            {
                Name = name,
                Author = author,
                IsHot = isHot,
                SefName = sefName,
                Icon = icon,
                Parent = parent
            };
        }

        public CategoryViewModel CreateCategoryViewModel(Category cat)
        {
            var categoryModel = new CategoryViewModel
            {
                Id = cat.Id,
                IsHot = cat.IsHot,
                ParentId = cat.Parent ?? -1,
                Name = cat.Name,
                Icon = cat.Icon,
                SefName = cat.SefName
            };
            return categoryModel;
        }
    }
}
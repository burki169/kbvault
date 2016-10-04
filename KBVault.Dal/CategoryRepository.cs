using System;
using System.Linq;

namespace KBVault.Dal
{
    public class CategoryRepository : ICategoryRepository
    {
        public int Add(Category category)
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return category.Id;
            }
        }

        public void Update(Category category)
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                Category cat = db.Categories.First(c => c.Id == category.Id);
                if (cat != null)
                {
                    cat.Name = category.Name;
                    cat.IsHot = category.IsHot;
                    cat.SefName = category.SefName;
                    cat.Author = category.Author;
                    cat.Parent = category.Parent;
                    db.SaveChanges();
                }
                else
                {
                    throw new ArgumentNullException();                    
                }
            }
        }

        public Category Get(int categoryId)
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {                
                var category = db.Categories.First(ca => ca.Id == categoryId);
                if (category == null)
                {
                    throw new ArgumentNullException("Category not found");
                }

                return category;
            }
        }
    }
}
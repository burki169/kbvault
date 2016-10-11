using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public IList<Category> GetAllCategories()
        {
            using (var db = new KbVaultEntities())
            {
                return db.Categories.OrderBy(c => c.Name).ToList();
            }
        }

        public bool HasArticleInCategory(int categoryId)
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                return db.Articles.Any(a => a.CategoryId == categoryId);
            }
        }

        public IList<Article> GetArticles(int categoryId)
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                return db.Articles.Include(a => a.KbUser).Include(a => a.Attachments).Where(a => a.CategoryId == categoryId).OrderBy(c => c.Title).ToList();
            }
        }

        public bool Remove(Category category)
        {            
            using (KbVaultEntities db = new KbVaultEntities())
            {
                var cat = db.Categories.FirstOrDefault(c => c.Id == category.Id);
                if (cat != null)
                {
                    cat.Author = category.Author;
                    db.Categories.Remove(cat);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        public Category GetFirstCategory()
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                return db.Categories.FirstOrDefault();
            }
        }
    }
}
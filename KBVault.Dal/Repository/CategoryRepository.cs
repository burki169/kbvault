using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KBVault.Dal.Entities;

namespace KBVault.Dal.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public int Add(Category category)
        {
            using (var db = new KbVaultContext())
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return category.Id;
            }
        }

        public void Update(Category category)
        {
            using (var db = new KbVaultContext())
            {
                var cat = db.Categories.FirstOrDefault(c => c.Id == category.Id);
                if (cat != null)
                {
                    cat.Name = category.Name;
                    cat.IsHot = category.IsHot;
                    cat.SefName = category.SefName;
                    cat.Author = category.Author;
                    cat.Parent = category.Parent;
                    cat.Icon = category.Icon;
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
            using (var db = new KbVaultContext())
            {
                var category = db.Categories.FirstOrDefault(ca => ca.Id == categoryId);
                if (category == null)
                {
                    throw new ArgumentNullException("Category not found");
                }

                return category;
            }
        }

        public IList<Category> GetAllCategories()
        {
            using (var db = new KbVaultContext())
            {
                return db.Categories.OrderBy(c => c.Name).ToList();
            }
        }

        public bool HasArticleInCategory(int categoryId)
        {
            using (var db = new KbVaultContext())
            {
                return db.Articles.Any(a => a.CategoryId == categoryId);
            }
        }

        public IList<Article> GetArticles(int categoryId)
        {
            using (var db = new KbVaultContext())
            {
                return db.Articles.Include(a => a.KbUser).Include(a => a.Attachments).Where(a => a.CategoryId == categoryId).OrderBy(c => c.Title).ToList();
            }
        }

        public bool Remove(Category category)
        {
            using (var db = new KbVaultContext())
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
            using (var db = new KbVaultContext())
            {
                return db.Categories.FirstOrDefault();
            }
        }

        public IList<Category> GetHotCategories()
        {
            using (var db = new KbVaultContext())
            {
                return db.Categories.Include("Articles").Where(c => c.IsHot).ToList();
            }
        }

        public IList<Category> GetFirstLevelCategories()
        {
            using (var db = new KbVaultContext())
            {
                return db.Categories.Include("Articles").Where(c => c.Parent == null).OrderBy(c => c.Name).ToList();
            }
        }
    }
}
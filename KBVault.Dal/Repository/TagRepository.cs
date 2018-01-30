using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using KBVault.Dal.Entities;
using KBVault.Dal.Types;

namespace KBVault.Dal.Repository
{
    public class TagRepository : ITagRepository
    {
        public IList<TopTagItem> GetTagCloud()
        {
            var popularTags = GetTopTags().OrderBy(c => Guid.NewGuid()).ToList();
            var maxTagRatio = popularTags.Max(t => t.Ratio).HasValue ? Convert.ToInt32(popularTags.Max(t => t.Ratio).Value) : -1;
            var minTagRatio = popularTags.Min(t => t.Ratio).HasValue ? Convert.ToInt32(popularTags.Min(t => t.Ratio).Value) : -1;
            var ratioDiff = maxTagRatio - minTagRatio;
            var minRatio = minTagRatio;
            foreach (var item in popularTags)
            {
                if (ratioDiff > 0)
                {
                    item.FontSize = 80 + Convert.ToInt32(Math.Truncate((double)(item.Ratio - minRatio) * (100 / ratioDiff)));
                }
                else
                {
                    item.FontSize = 80;
                }
            }

            return popularTags;
        }

        public IList<TopTagItem> GetTopTags()
        {
            using (var db = new KbVaultContext())
            {
                return db.Database.SqlQuery<TopTagItem>("exec GetTopTags").ToList();
            }
        }

        public void RemoveTagFromArticles(int tagId)
        {
            using (var db = new KbVaultContext())
            {
                var tagIdParam = new SqlParameter("TagId", tagId);
                db.Database.ExecuteSqlCommand("exec RemoveTagFromArticles @TagId", tagIdParam);
            }
        }
    }
}
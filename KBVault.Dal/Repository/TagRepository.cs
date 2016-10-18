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
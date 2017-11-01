using System.Collections.Generic;
using KBVault.Dal.Entities;
using KBVault.Dal.Types;

namespace KBVault.Dal.Repository
{
    public interface ITagRepository
    {
        void RemoveTagFromArticles(int tagId);
        IList<TopTagItem> GetTopTags();
        IList<TopTagItem> GetTagCloud();
    }
}
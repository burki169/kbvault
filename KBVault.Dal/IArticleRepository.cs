using System;
using System.Collections.Generic;

namespace KBVault.Dal
{
    public interface IArticleRepository
    {
        Article Get(long id);
        long Add(Article article,string tags);
        void Update(Article article, string tags);
    }
}
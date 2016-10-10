using System;
using System.Collections.Generic;

namespace KBVault.Dal
{
    public interface IArticleRepository
    {
        Article Get(long id);
        long Add(Article article,string tags);
    }
}
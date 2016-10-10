using System.Linq;

namespace KBVault.Dal
{
    public class UserRepository : IUserRepository
    {
        public KbUser Get(long id)
        {
            using (var db = new KbVaultEntities())
            {
                return db.KbUsers.First(u => u.Id == id);
            }
        }
    }
}
using System.Linq;

namespace KBVault.Dal.Repository
{
    public class UserRepository : IUserRepository
    {
        public Entities.KbUser Get(long id)
        {
            using (var db = new KbVaultContext())
            {
                return db.KbUsers.First(u => u.Id == id);
            }
        }
    }
}
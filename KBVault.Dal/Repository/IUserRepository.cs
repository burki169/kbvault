namespace KBVault.Dal.Repository
{
    public interface IUserRepository
    {
        Entities.KbUser Get(long id);
    }
}
namespace KBVault.Dal
{
    public interface ICategoryRepository
    {
        int Add(Category category);
        void Update(Category category);
        Category Get(int categoryId);
    }
}
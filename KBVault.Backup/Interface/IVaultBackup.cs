namespace KBVault.Backup.Interface
{
    public interface IVaultBackup
    {
        void Connect(string connectionString);
        bool Backup(string databaseName, string physicalPath);
        bool Restore(string databaseName, string physicalPath);
    }
}

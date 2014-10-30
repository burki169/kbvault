using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBVault.Backup.Interface
{
    public delegate bool VaultBackupProgress( int progress );
    
    public interface IVaultBackup
    {
        void ConnectToNamedInstance(string instanceName, string userName, string password, bool useWindowsAuthentication);
        void ConnectToRemoteInstance(string instanceName, string userName, string password, bool useWindowsAuthentication);
        void BackupTo(string databaseName, string physicalPath, VaultBackupProgress progressFunction );
    }
}

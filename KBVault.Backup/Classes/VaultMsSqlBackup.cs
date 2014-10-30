using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBVault.Backup.Interface;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace KBVault.Backup.Classes
{
    public class VaultMsSqlBackup: IVaultBackup
    {
        private string ServerName;
        private string UserName;
        private string Password;
        private bool UseWindowsAuthentication;
        private Server SqlServer = null;

        public void ConnectToNamedInstance(string instanceName, string userName, string password, bool useWindowsAuthentication)
        {
            ServerName = instanceName;
            UserName = userName;
            Password = password;
            UseWindowsAuthentication = useWindowsAuthentication;
            ServerConnection srvConn = new ServerConnection();
            srvConn.ServerInstance = @".\" + ServerName; 
            srvConn.LoginSecure = UseWindowsAuthentication;
            srvConn.Login = UserName;
            srvConn.Password = Password;
            SqlServer = new Server(srvConn);
            string version = SqlServer.Information.Version.Major.ToString();

            
        }

        public void BackupTo(string databaseName, string physicalPath, VaultBackupProgress progressFunction)
        {
            if (SqlServer == null)
                throw new ArgumentNullException("Sql Server Object is null. Call connect first");
        }

        
        public void ConnectToRemoteInstance(string serverName, string userName, string password, bool useWindowsAuthentication)
        {
            throw new NotImplementedException();
        }
    }
}

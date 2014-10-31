using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBVault.Backup.Interface;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SmoBackup = Microsoft.SqlServer.Management.Smo.Backup;

namespace KBVault.Backup.Classes
{
    public class VaultMsSqlBackup: IVaultBackup
    {
        private string ServerName;
        private string UserName;
        private string Password;
        private bool UseWindowsAuthentication;
        private Server SqlServer = null;
        private VaultBackupProgress ClientProgressFunction;

        private void BackupProgressEvent(object sender, PercentCompleteEventArgs e)
        {
            ClientProgressFunction(e.Percent);
        }

        private void Connect(string instanceName, string userName, string password, bool useWindowsAuthentication)
        {
            ServerName = instanceName;
            UserName = userName;
            Password = password;
            UseWindowsAuthentication = useWindowsAuthentication;
            ServerConnection srvConn = new ServerConnection();
            srvConn.ServerInstance = ServerName;
            srvConn.LoginSecure = UseWindowsAuthentication;
            if (!srvConn.LoginSecure)
            {
                srvConn.Login = UserName;
                srvConn.Password = Password;
            }
            SqlServer = new Server(srvConn);
            SqlServer.ConnectionContext.ConnectTimeout = 5;
            SqlServer.ConnectionContext.StatementTimeout = 5;
        }

        public void ConnectToRemoteInstance(string instanceName, string userName, string password, bool useWindowsAuthentication)
        {
            Connect(instanceName, userName, password, useWindowsAuthentication);
        }

        public void ConnectToNamedInstance(string instanceName, string userName, string password, bool useWindowsAuthentication)
        {            
            Connect(instanceName, userName, password, useWindowsAuthentication);
        }

        public async Task<bool> Backup(string databaseName, string physicalPath, VaultBackupProgress progressFunction)
        {
            if (SqlServer == null)
                throw new ArgumentNullException("Sql Server Object is null. Call connect first");
            ClientProgressFunction = progressFunction;
            SmoBackup backup = new SmoBackup();
            backup.Action = BackupActionType.Database;
            backup.Database = databaseName;
            
            backup.Devices.AddDevice(physicalPath, DeviceType.File);
            backup.PercentCompleteNotification = 10;
            backup.PercentComplete += new PercentCompleteEventHandler(BackupProgressEvent);
            backup.SqlBackup(SqlServer);
            return true;
        }

        public async Task<bool> Restore(string databaseName, string physicalPath, VaultBackupProgress progressFunction)
        {
            Restore restore = new Restore();
            restore.Action = RestoreActionType.Database;
            restore.Devices.AddDevice(physicalPath, DeviceType.File);
            restore.Database = databaseName;
            restore.PercentCompleteNotification = 10;
            restore.PercentComplete += new PercentCompleteEventHandler(BackupProgressEvent);
            restore.SqlRestore(SqlServer);
            return true;
        }
    }
}

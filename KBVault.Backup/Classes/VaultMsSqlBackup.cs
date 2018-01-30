using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBVault.Backup.Interface;

namespace KBVault.Backup.Classes
{
    public class VaultMsSqlBackup: IVaultBackup
    {
        private string ConnectionString;

        public void Connect(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool Backup(string databaseName, string physicalPath)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BACKUP DATABASE " + databaseName+" ");
                sb.AppendLine("TO DISK='" + physicalPath + "'");
                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool Restore(string databaseName, string physicalPath)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("USE [master]");
                sb.AppendLine("ALTER DATABASE " + databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                sb.AppendLine("RESTORE DATABASE " + databaseName + " FROM DISK='" + physicalPath + "'");
                sb.AppendLine("WAITFOR DELAY '00:00:02'");
                sb.AppendLine("ALTER DATABASE " + databaseName + " SET MULTI_USER");
                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();
            }
            return true;
        }
    }
}

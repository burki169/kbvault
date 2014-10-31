using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBVault.Backup.Classes;
using KBVault.Backup.Interface;

namespace BackupTestApplication
{
    class Program
    {
        static bool Progress(int perc)
        {
            Console.WriteLine("{0} % completed", perc);
            return true;
        }
        static void Main(string[] args)
        {
            IVaultBackup backup = new VaultMsSqlBackup();
            backup.ConnectToNamedInstance("sqlexpress", "", "", true);
            backup.Backup("Licomas", "d:\\Licomas.bak", Progress);
            backup.Restore("Licomas", "d:\\Licomas.bak", Progress);
            Console.ReadLine();
        }
    }
}

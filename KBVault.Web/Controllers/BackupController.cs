using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KBVault.Backup;
using KBVault.Backup.Classes;
using KBVault.Backup.Interface;
using KBVault.Web.Models;
using Resources;

namespace KBVault.Web.Controllers
{
    public class BackupController : KbVaultAdminController
    {
        private const string BackupPath = "~/BackupArchives/";

        public ActionResult Index()
        {
            List<BackupListViewModel> model = new List<BackupListViewModel>();
            string BackupDirectory = Server.MapPath(BackupPath);
            int i = 0;
            foreach (var filePath in Directory.GetFiles(BackupDirectory, "*.bak"))
            {
                FileInfo fo = new FileInfo(filePath);
                model.Add(new BackupListViewModel()
                {
                    Id = i,
                    FileName = Path.GetFileName(filePath),
                    FileDate = fo.CreationTime
                });
                i++;
            }
            return View(model.OrderByDescending( f => f.FileDate));
        }


        [HttpPost]
        public JsonResult Restore(string file)
        {
            JsonOperationResponse result = new JsonOperationResponse();            
            try
            {
                string backupFile = Server.MapPath(BackupPath + file);
                if( System.IO.File.Exists(backupFile) )
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["KbVaultEntities"].ConnectionString;
                    EntityConnectionStringBuilder entityConnectionString = new EntityConnectionStringBuilder(connectionString);
                    System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(entityConnectionString.ProviderConnectionString);

                    IVaultBackup backup = new VaultMsSqlBackup();
                    backup.Connect(entityConnectionString.ProviderConnectionString);
                    result.Successful = backup.Restore(builder.InitialCatalog, backupFile);
                    if (result.Successful)
                        result.ErrorMessage = UIResources.BackupRestoreSuccessfull;
                }
                else{
                    throw new FileNotFoundException(file);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.ErrorMessage = ex.Message;                
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult BackupNow()
        {
            JsonOperationResponse result = new JsonOperationResponse();            
            
            string connectionString =  ConfigurationManager.ConnectionStrings["KbVaultEntities"].ConnectionString;
            EntityConnectionStringBuilder entityConnectionString = new EntityConnectionStringBuilder(connectionString);
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(entityConnectionString.ProviderConnectionString);             

            IVaultBackup backup = new VaultMsSqlBackup();
            backup.Connect(entityConnectionString.ProviderConnectionString);
            string backupFile = String.Format("{0:yyyyMddhhmm}.bak", DateTime.Now);
            bool b = backup.Backup(builder.InitialCatalog, Server.MapPath(BackupPath + backupFile));
            if (b)
            {
                if (!String.IsNullOrEmpty(backupFile))
                {
                    result.Data = backupFile;
                    result.Successful = true;
                }
            }
            return Json(result,JsonRequestBehavior.DenyGet);
        }

    }
}

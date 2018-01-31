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
        public ActionResult Index()
        {
            var backupDirectory = string.Empty;
            if (!string.IsNullOrEmpty(Settings.BackupPath) && Settings.BackupPath.StartsWith("~"))
            {
                backupDirectory = Server.MapPath(Settings.BackupPath);
            }
            else
            {
                backupDirectory = Settings.BackupPath;
            }

            var model = new List<BackupListViewModel>();
            var i = 0;
            if (!string.IsNullOrEmpty(backupDirectory))
            {
                foreach (var filePath in Directory.GetFiles(backupDirectory, "*.bak"))
                {
                    var fo = new FileInfo(filePath);
                    model.Add(new BackupListViewModel
                    {
                        Id = i,
                        FileName = Path.GetFileName(filePath),
                        FileDate = fo.CreationTime
                    });
                    i++;
                }
            }

            return View(model.OrderByDescending(f => f.FileDate));
        }

        [HttpPost]
        public JsonResult Restore(string file)
        {
            try
            {
                var result = new JsonOperationResponse();
                try
                {
                    var backupFile = string.Empty;
                    if (Settings.BackupPath.StartsWith("~"))
                    {
                        backupFile = Server.MapPath(Settings.BackupPath + file);
                    }
                    else
                    {
                        backupFile = Settings.BackupPath + file;
                    }

                    if (System.IO.File.Exists(backupFile))
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings["KbVaultEntities"].ConnectionString;
                        var entityConnectionString = new EntityConnectionStringBuilder(connectionString);
                        var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(entityConnectionString.ProviderConnectionString);

                        IVaultBackup backup = new VaultMsSqlBackup();
                        backup.Connect(entityConnectionString.ProviderConnectionString);
                        result.Successful = backup.Restore(builder.InitialCatalog, backupFile);
                        if (result.Successful)
                        {
                            result.ErrorMessage = UIResources.BackupRestoreSuccessfull;
                        }
                    }
                    else
                    {
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
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        [HttpPost]
        public JsonResult BackupNow()
        {
            try
            {
                var result = new JsonOperationResponse();
                if (string.IsNullOrEmpty(Settings.BackupPath))
                {
                    result.Successful = false;
                    result.ErrorMessage = ErrorMessages.BackupPathIsNotSet;
                }
                else
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["KbVaultContext"].ConnectionString;
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                    IVaultBackup backup = new VaultMsSqlBackup();
                    backup.Connect(connectionString);
                    var backupFile = string.Format("{0:yyyyMddhhmm}.bak", DateTime.Now);
                    if (!string.IsNullOrEmpty(Settings.BackupPath) && Settings.BackupPath.StartsWith("~"))
                    {
                        backupFile = Server.MapPath(Settings.BackupPath + backupFile);
                    }
                    else
                    {
                        backupFile = Settings.BackupPath + backupFile;
                    }

                    var backupSuccessful = backup.Backup(builder.InitialCatalog, backupFile);
                    if (backupSuccessful)
                    {
                        if (!string.IsNullOrEmpty(backupFile))
                        {
                            result.Data = backupFile;
                            result.Successful = true;
                        }
                    }
                }

                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}

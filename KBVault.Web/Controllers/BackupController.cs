using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KBVault.Backup;
using KBVault.Backup.Classes;
using KBVault.Backup.Interface;
using KBVault.Web.Models;

namespace KBVault.Web.Controllers
{
    public class BackupController : Controller
    {
        //
        // GET: /Backup/

        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        public bool SendMessageToClients(int percentage)
        {
            int i = percentage;
            return true;
        }


        [HttpPost]
        public async Task<JsonResult> BackupNow()
        {
            JsonOperationResponse result = new JsonOperationResponse();            
            
            string connectionString =  ConfigurationManager.ConnectionStrings["KbVaultEntities"].ConnectionString;
            EntityConnectionStringBuilder entityConnectionString = new EntityConnectionStringBuilder(connectionString);
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(entityConnectionString.ProviderConnectionString);             

            IVaultBackup backup = new VaultMsSqlBackup();
            backup.ConnectToNamedInstance(builder.DataSource, builder.UserID, builder.Password, builder.IntegratedSecurity);
            string backupFile = String.Format("{0:yyyyMddhhmm}.bak", DateTime.Now);
            bool b = await backup.Backup(builder.InitialCatalog, Server.MapPath("~/Backup/" + backupFile), SendMessageToClients);
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

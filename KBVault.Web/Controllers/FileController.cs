using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Helpers;
using KBVault.Web.Models;
using KBVault.Web.Resources;
using NLog;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class FileController : Controller
    {

        private Logger Log = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public JsonResult Remove(string id)
        {
            JsonOperationResponse result = new JsonOperationResponse();
            result.Successful = false;
            try
            {
                Attachment at = new Attachment() { Id = Convert.ToInt64(id) };                
                KbVaultAttachmentHelper.RemoveAttachment(id);
                KbVaultLuceneHelper.RemoveAttachmentFromIndex(at);
                result.Successful = true;
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult Upload()
        {
            JsonOperationResponse result = new JsonOperationResponse();
            result.Successful = false;
            try
            {
                if (Request.Params["ArticleId"] == null)
                {
                    result.ErrorMessage = ErrorMessages.FileUploadArticleNotFound;
                }                                
                else if (Request.Files.Count == 1)
                {
                    long articleId = Convert.ToInt64(Request.Params["ArticleId"]);
                    HttpPostedFileBase attachedFile = Request.Files[0];
                    Attachment attachment = KbVaultAttachmentHelper.SaveAttachment(articleId, attachedFile);
                    result.Successful = true;
                    result.Data = new AttachmentViewModel(attachment);
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        Setting sets = db.Settings.FirstOrDefault();
                        if (sets != null)
                        {
                            string[] extensions = sets.IndexFileExtensions.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries);

                            if (extensions.FirstOrDefault(a => a == attachment.Extension) != null )
                            {
                                KbVaultLuceneHelper.AddAttachmentToIndex(attachment);
                            }
                        }
                        
                    }
                }
                else
                {
                    result.ErrorMessage = ErrorMessages.FileUploadTooManyFiles;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

    }
}

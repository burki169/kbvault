using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Web.Helpers;
using KBVault.Web.Models;
using NLog;
using Resources;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private Logger log = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public JsonResult Remove(string id)
        {
            var result = new JsonOperationResponse
            {
                Successful = false
            };
            try
            {
                var parts = id.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    var attachmentHash = parts[0];
                    var attachmentId = parts[1];

                    var at = new Attachment
                    {
                        Id = Convert.ToInt64(attachmentId),
                        Author = KBVaultHelperFunctions.UserAsKbUser(User).Id
                    };

                    KbVaultAttachmentHelper.RemoveAttachment(attachmentHash, KBVaultHelperFunctions.UserAsKbUser(User).Id);
                    KbVaultLuceneHelper.RemoveAttachmentFromIndex(at);
                    result.Successful = true;
                    return Json(result);
                }

                throw new ArgumentOutOfRangeException("Invalid file hash");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var result = new JsonOperationResponse
            {
                Successful = false
            };
            try
            {
                if (Request.Params["ArticleId"] == null)
                {
                    result.ErrorMessage = ErrorMessages.FileUploadArticleNotFound;
                }
                else if (Request.Files.Count == 1)
                {
                    var articleId = Convert.ToInt64(Request.Params["ArticleId"]);
                    var attachedFile = Request.Files[0];
                    var attachment = KbVaultAttachmentHelper.SaveAttachment(articleId, attachedFile, KBVaultHelperFunctions.UserAsKbUser(User).Id);
                    attachment.Author = KBVaultHelperFunctions.UserAsKbUser(User).Id;
                    result.Successful = true;
                    result.Data = new AttachmentViewModel(attachment);
                    using (var db = new KbVaultContext())
                    {
                        var sets = db.Settings.FirstOrDefault();
                        if (sets != null)
                        {
                            var extensions = sets.IndexFileExtensions.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            if (extensions.FirstOrDefault(a => a.ToLowerInvariant() == attachment.Extension.ToLowerInvariant()) != null)
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
                log.Error(ex);
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }
    }
}

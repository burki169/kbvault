using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using KBVault.Dal;
using KBVault.Dal.Entities;
using NLog;
using Resources;

namespace KBVault.Web.Helpers
{
    public class KbVaultAttachmentHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void RemoveLocalAttachmentFile(Attachment at)
        {
            try
            {
              var localPath = Path.Combine(HttpContext.Current.Server.MapPath(at.Path), at.FileName);
              System.IO.File.Delete(localPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void RemoveAttachment(string hash, long currentUserId)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var attachment = db.Attachments.First(a => a.Hash == hash);
                    if (attachment == null)
                    {
                        throw new ArgumentNullException(ErrorMessages.AttachmentNotFound);
                    }

                    var localPath = Path.Combine(HttpContext.Current.Server.MapPath(attachment.Path), attachment.FileName);
                    attachment.Author = currentUserId;
                    db.Attachments.Remove(attachment);
                    db.SaveChanges();
                    File.Delete(localPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static Attachment SaveAttachment(long articleId, HttpPostedFileBase attachedFile, long userId)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                    var article = db.Articles.First(a => a.Id == articleId);
                    if (article != null)
                    {
                        var attachment = new Attachment();
                        var localPath = HttpContext.Current.Server.MapPath("~/Uploads");
                        attachment.Path = "~/Uploads/";
                        attachment.FileName = Path.GetFileName(attachedFile.FileName);
                        attachment.Extension = Path.GetExtension(attachedFile.FileName);
                        attachment.ArticleId = articleId;
                        attachment.MimeType = attachedFile.ContentType;
                        attachment.Hash = Guid.NewGuid().ToString().Replace("-", string.Empty);
                        attachment.Author = userId;
                        db.Attachments.Add(attachment);
                        article.Attachments.Add(attachment);

                        var path = Path.Combine(localPath, attachment.FileName);
                        while (System.IO.File.Exists(path))
                        {
                            attachment.FileName = Path.GetFileNameWithoutExtension(attachment.FileName) +
                                                   Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(1, 5) +
                                                   Path.GetExtension(attachment.FileName);
                            path = Path.Combine(localPath, attachment.FileName);
                        }

                        attachedFile.SaveAs(path);
                        db.SaveChanges();
                        return attachment;
                    }

                    throw new ArgumentNullException(ErrorMessages.FileUploadArticleNotFound);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
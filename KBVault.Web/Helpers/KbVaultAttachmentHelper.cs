using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using KBVault.Dal;
using KBVault.Web.Resources;
using NLog;

namespace KBVault.Web.Helpers
{
    public class KbVaultAttachmentHelper
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static void RemoveAttachment(string hash)
        {
            try
            {
                using( KbVaultEntities db = new KbVaultEntities())
                {                    
                    Attachment attachment = db.Attachments.First(a => a.Hash == hash);
                    if (attachment == null)
                        throw new ArgumentNullException(ErrorMessages.AttachmentNotFound);
                    string localPath = Path.Combine( HttpContext.Current.Server.MapPath(attachment.Path), attachment.FileName);
                    db.Attachments.Remove(attachment);
                    db.SaveChanges();
                    System.IO.File.Delete(localPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static Attachment SaveAttachment(long articleId, HttpPostedFileBase attachedFile)
        {
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;
                    Article article = db.Articles.First(a => a.Id == articleId);
                    if (article != null)
                    {
                        
                        Attachment attachment = new Attachment();
                        // think of organizing in year/month folders
                        string localPath = HttpContext.Current.Server.MapPath("~/Uploads");
                        attachment.Path = "~/Uploads/";
                        attachment.FileName = Path.GetFileName(attachedFile.FileName);
                        attachment.Extension = Path.GetExtension(attachedFile.FileName);
                        attachment.ArticleId = articleId;
                        attachment.MimeType = attachedFile.ContentType;
                        attachment.Hash = Guid.NewGuid().ToString().Replace("-", "");
                        db.Attachments.Add(attachment);
                        article.Attachments.Add(attachment);

                        string path = Path.Combine(localPath, attachment.FileName);
                        while (System.IO.File.Exists(path))
                        {
                            attachment.FileName = Path.GetFileNameWithoutExtension(attachment.FileName) +
                                                   Guid.NewGuid().ToString().Replace("-", "").Substring(1, 5) +
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
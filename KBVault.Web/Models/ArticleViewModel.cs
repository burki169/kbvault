using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Resources;

namespace KBVault.Web.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
        }
        
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public bool IsDraft { get; set; }
        public string Tags { get; set; }
        [Required(ErrorMessageResourceType = typeof(KBVault.Web.Resources.UIResources), ErrorMessageResourceName = "ArticleCreatePagePublishStartRequiredMessage")]
        public DateTime PublishStartDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(KBVault.Web.Resources.UIResources), ErrorMessageResourceName = "ArticleCreatePagePublishEndRequiredMessage")]
        public DateTime PublishEndDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "ArticleSefNameIsRequired")]
        public string SefName { get; set; }
        public KbUser Author { get; set; }
        public List<AttachmentViewModel> Attachments = new List<AttachmentViewModel>();        
        public CategoryViewModel Category { get; set; }
    }
}
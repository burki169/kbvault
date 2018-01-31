using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;

using Resources;
using KbUser = KBVault.Dal.Entities.KbUser;

namespace KBVault.Web.Models
{
    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
            Attachments = new List<AttachmentViewModel>();
        }

        public List<AttachmentViewModel> Attachments { get; set; }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public bool IsDraft { get; set; }
        public string Tags { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "ArticleCreatePagePublishStartRequiredMessage")]
        public DateTime PublishStartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "ArticleCreatePagePublishEndRequiredMessage")]
        public DateTime PublishEndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIResources), ErrorMessageResourceName = "ArticleSefNameIsRequired")]
        public string SefName { get; set; }
        public KbUser Author { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}
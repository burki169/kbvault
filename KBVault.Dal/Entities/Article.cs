using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Entities
{
    [Table("Article")]
    public partial class Article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Article()
        {
            ArticleTags = new HashSet<ArticleTag>();
            Attachments = new HashSet<Attachment>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public int Views { get; set; }

        public int Likes { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Edited { get; set; }

        public int IsDraft { get; set; }

        public DateTime? PublishStartDate { get; set; }

        public DateTime? PublishEndDate { get; set; }

        public long Author { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string SefName { get; set; }

        public virtual KbUser KbUser { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}

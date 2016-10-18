using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Entities
{
    [Table("Attachment")]
    public partial class Attachment
    {
        public long Id { get; set; }

        public long ArticleId { get; set; }

        [Required]
        [StringLength(2048)]
        public string Path { get; set; }

        [Required]
        [StringLength(2048)]
        public string FileName { get; set; }

        [Required]
        [StringLength(5)]
        public string Extension { get; set; }

        public int Downloads { get; set; }

        [StringLength(256)]
        public string Hash { get; set; }

        public DateTime? HashTime { get; set; }

        [StringLength(50)]
        public string MimeType { get; set; }

        public long Author { get; set; }

        public virtual Article Article { get; set; }

        public virtual KbUser KbUser { get; set; }
    }
}

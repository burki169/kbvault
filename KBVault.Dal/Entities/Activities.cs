using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Entities
{
    public partial class Activities
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ActivityDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Operation { get; set; }

        [StringLength(500)]
        public string Information { get; set; }

        public virtual KbUser KbUser { get; set; }
    }
}

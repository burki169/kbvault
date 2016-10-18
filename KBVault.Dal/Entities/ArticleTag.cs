using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Entities
{
    [Table("ArticleTag")]
    public partial class ArticleTag
    {
        public long Id { get; set; }

        public long TagId { get; set; }

        public long ArticleId { get; set; }

        public long Author { get; set; }

        public virtual Article Article { get; set; }

        public virtual KbUser KbUser { get; set; }

        public virtual Tag Tag { get; set; }
    }
}

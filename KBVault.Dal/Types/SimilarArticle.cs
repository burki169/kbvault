using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Types
{
    [NotMapped]
    public class SimilarArticle
    {
        public long? Id { get; set; }
        public string SefName { get; set; }
        public string Title { get; set; }
        public DateTime PublishEndDate { get; set; }
        public DateTime PublishStartDate { get; set; }
        public int IsDraft { get; set; }
        public int Relevance { get; set; }
    }
}

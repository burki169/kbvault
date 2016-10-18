using System.ComponentModel.DataAnnotations.Schema;

namespace KBVault.Dal.Types
{
    [NotMapped]
    public class TopTagItem
    {
        public int? Ratio { get; set; }
        public string Name { get; set; }
        public long? Id { get; set; }
        public int FontSize { get; set; }
    }
}

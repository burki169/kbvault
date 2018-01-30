using System;

namespace KBVault.Web.Models
{
    public class BackupListViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime FileDate { get; set; }
    }
}
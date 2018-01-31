using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KBVault.Web.Models
{
    public class ActivityViewModel
    {
        public string ActivityDate { get; set; }
        public string Operation { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
    }
}
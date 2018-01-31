using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KBVault.Web.Models
{
    public class ActivityDataTablesPostModel
    {
#pragma warning disable SA1300 // Element must begin with upper-case letter
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
#pragma warning restore SA1300 // Element must begin with upper-case letter
    }
}
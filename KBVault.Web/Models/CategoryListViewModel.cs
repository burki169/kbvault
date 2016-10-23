using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KBVault.Dal;
using KBVault.Dal.Entities;
using MvcPaging;

namespace KBVault.Web.Models
{
    public class CategoryListViewModel
    {
        public string CategoryName { get; set; }
        public string Icon { get; set; }
        public int CategoryId { get; set; }
        public IPagedList<Article> Articles { get; set; }
    }
}
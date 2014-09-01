using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KBVault.Dal;
using KBVault.Web.Resources;

namespace KBVault.Web.Models
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
        }
        public CategoryViewModel(Category cat)
        {
            this.Id = cat.Id;
            this.IsHot = cat.IsHot;
            this.Name = cat.Name;
            this.ParentId = cat.Parent??-1;            
        }
        public int Id { get; set; }

        [Required(ErrorMessageResourceType=typeof(Resources.ErrorMessages), ErrorMessageResourceName="CategoryNameIsRequired")]
        public string Name { get; set; }        
        
        public bool IsHot { get; set; }
        public int ParentId { get; set; }
        public string NameForDroplist { get; set; }
        
        [Required(ErrorMessageResourceType=typeof(UIResources),ErrorMessageResourceName="CategorySefNameIsRequired")]
        public string SefName { get; set; }
        
        public List<CategoryViewModel> Children = new List<CategoryViewModel>();            
    }
}
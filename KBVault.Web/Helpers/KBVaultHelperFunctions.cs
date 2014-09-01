using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using KBVault.Core.MVC.Authorization;
using KBVault.Dal;
using KBVault.Web.Models;
using NLog;

namespace KBVault.Web.Helpers
{
    public class KBVaultHelperFunctions
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static KbUser UserAsKbUser(IPrincipal user)
        {
            try
            {
                if( HttpContext.Current.Request.IsAuthenticated )
                {
                    
                    return KbVaultAuthHelper.GetKbUser(user.Identity.Name);
                }
                throw new ArgumentNullException("Identity is null");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

      
        private static string GetCategoryMenu( long parentCategoryId = -1/*, long activeCategory = -1*/)
        {
            try
            {
                string html ="";
                UrlHelper linkHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                List<CategoryViewModel> categoryTree = GetCategories(parentCategoryId,0,false);                
                var activeClass = "active";                
                html = "<ul class='treeview-menu " + activeClass + "'>";

                foreach (CategoryViewModel model in categoryTree)
                {
                    html += "<li class='treeview'>" + Environment.NewLine.ToString();
                    html += "<div>" + Environment.NewLine.ToString();
                    string categoryArticleListLink = linkHelper.Action("List", "Category", new { id = model.Id, page = 1 });
                    html += String.Format("<a href='{0}'>", categoryArticleListLink);
                    html += "<i class='fa fa-angle-double-right'></i> " + model.Name;
                    html += "</a>" + Environment.NewLine.ToString();
                    //html += "<i class='fa fa-angle-left pull-right trigger-item'></i>" + Environment.NewLine.ToString();
                    html += "</div>" + Environment.NewLine.ToString();
                    if (model.Children.Count > 0)
                    {
                        html += GetCategoryMenu(model.Id);
                    }
                    html += "</li>" + Environment.NewLine.ToString();
                }
                html += "</ul>";
                
                return html;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        private static List<CategoryViewModel> GetCategories(long parentCategoryId = -1, int depth = 0, bool createSingleListForDropdown = true)
        {
            try
            {
                List<CategoryViewModel> CategoryList = new List<CategoryViewModel>();
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    //db.Configuration.ProxyCreationEnabled = false;
                    List<Category> categories = db.Categories.Where(c => c.Parent == parentCategoryId || (parentCategoryId == -1 && c.Parent == null) ).ToList();
                    foreach (Category cat in categories)
                    {
                        CategoryViewModel categoryItem = new CategoryViewModel();
                        categoryItem.Id = cat.Id;
                        categoryItem.Name = cat.Name;
                        categoryItem.SefName = cat.SefName;
                        categoryItem.NameForDroplist = cat.Name.PadLeft(cat.Name.Length + depth, '-');
                        categoryItem.Children = GetCategories(cat.Id, depth + 2);
                        CategoryList.Add(categoryItem);
                        if( createSingleListForDropdown )
                            CategoryList.AddRange(categoryItem.Children);
                    }
                }
                return CategoryList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static MvcHtmlString CreateCategoryMenu(/*long activeCategory*/)
        {
            return new MvcHtmlString(GetCategoryMenu(-1));
        }

        public static MvcHtmlString CreateBootstrapCategoryMenu()
        {
            return new MvcHtmlString(GetBootstrapCategoryMenu(-1));
        }

        private static string GetBootstrapCategoryMenu(long parentCategoryId = -1/*, long activeCategory = -1*/)
        {
            try
            {
                string html = "";
                UrlHelper linkHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                List<CategoryViewModel> categoryTree = GetCategories(parentCategoryId, 0, false);                
                if( parentCategoryId == -1 )
                    html = "<ul class=\"dropdown-menu multi-level\" role=\"menu\" aria-labelledby=\"dropdownMenu\">";

                foreach (CategoryViewModel model in categoryTree)
                {
                    if( model.Children.Count > 0 )
                        html += "<li class=\"dropdown-submenu pull-left\">" + Environment.NewLine.ToString();
                    else 
                        html += "<li>"+ Environment.NewLine.ToString();

                    string categoryListLink = linkHelper.Action("Categories", "Home", new { id = model.SefName});
                    html += String.Format("<a href='{0}'>{1}</a>", categoryListLink,model.Name);
                    html += Environment.NewLine.ToString();                                        
                    if (model.Children.Count > 0)
                    {
                        html += "<ul class=\"dropdown-menu\">";
                        html += GetBootstrapCategoryMenu(model.Id);
                        html += "</ul>";
                    }
                    html += "</li>" + Environment.NewLine.ToString();
                }
                if( parentCategoryId == -1 )
                    html += "</ul>";

                return html;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

       

        public static SelectList CategoryTreeForSelectList(long selectedCategoryId,bool displayRoot = true)
        {
            try
            {
                List<CategoryViewModel> cats = new List<CategoryViewModel>();
                CategoryViewModel root = new CategoryViewModel();
                if (displayRoot)
                {
                    root.Id = -1;
                    root.NameForDroplist = " ";
                    cats.Add(root);
                }
                cats.AddRange( GetCategories(-1) );
                return new SelectList(cats, "Id", "NameForDroplist", selectedCategoryId);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static SelectList CreateRolesDropdown(string selectedRole)
        {
            try
            {
                List<object> objs = new List<object>();
                objs.Add(new { Value = KbVaultAuthHelper.ROLE_ADMIN, Text = KbVaultAuthHelper.ROLE_ADMIN });
                objs.Add(new { Value = KbVaultAuthHelper.ROLE_MANAGER, Text = KbVaultAuthHelper.ROLE_MANAGER});
                objs.Add(new { Value = KbVaultAuthHelper.ROLE_EDITOR, Text = KbVaultAuthHelper.ROLE_EDITOR});

                return new SelectList(objs, "Value", "Text", selectedRole);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static bool IsAdmin(IPrincipal user)
        {
            try
            {
                KbUser usr = UserAsKbUser(user);
                return usr.Role == KbVaultAuthHelper.ROLE_ADMIN;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static bool IsManager(IPrincipal user)
        {
            try
            {
                KbUser usr = UserAsKbUser(user);
                return usr.Role == KbVaultAuthHelper.ROLE_MANAGER;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
             
    }
}
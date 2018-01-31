using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using KBVault.Core.MVC.Authorization;
using KBVault.Dal;
using KBVault.Dal.Entities;
using KBVault.Web.Models;
using NLog;
using KbUser = KBVault.Dal.Entities.KbUser;

namespace KBVault.Web.Helpers
{
    public class KBVaultHelperFunctions
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static KbUser UserAsKbUser(IPrincipal user)
        {
            try
            {
                if (HttpContext.Current.Request.IsAuthenticated)
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

        public static MvcHtmlString CreateCategoryMenu()
        {
            return new MvcHtmlString(GetCategoryMenu(-1));
        }

        public static MvcHtmlString CreateBootstrapCategoryMenu()
        {
            return new MvcHtmlString(GetBootstrapCategoryMenu(-1));
        }

        public static SelectList CategoryTreeForSelectList(long selectedCategoryId, bool displayRoot = true)
        {
            try
            {
                var cats = new List<CategoryViewModel>();
                var root = new CategoryViewModel();
                if (displayRoot)
                {
                    root.Id = -1;
                    root.NameForDroplist = " ";
                    cats.Add(root);
                }

                cats.AddRange(GetCategories(-1));
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
                var objs = new List<object>
                {
                    new { Value = KbVaultAuthHelper.RoleAdmin, Text = KbVaultAuthHelper.RoleAdmin },
                    new { Value = KbVaultAuthHelper.RoleManager, Text = KbVaultAuthHelper.RoleManager },
                    new { Value = KbVaultAuthHelper.RoleEditor, Text = KbVaultAuthHelper.RoleEditor }
                };

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
                var usr = UserAsKbUser(user);
                return usr.Role == KbVaultAuthHelper.RoleAdmin;
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
                var usr = UserAsKbUser(user);
                return usr.Role == KbVaultAuthHelper.RoleManager;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        private static string GetCategoryMenu(long parentCategoryId = -1)
        {
            try
            {
                var linkHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var categoryTree = GetCategories(parentCategoryId, 0, false);
                var activeClass = "active";
                var html = new StringBuilder();
                html.Append($"<ul class='treeview-menu {activeClass}'>");

                foreach (var model in categoryTree)
                {
                    var categoryArticleListLink = linkHelper.Action("List", "Category", new { id = model.Id, page = 1 });
                    html.Append("<li class='treeview'>" + Environment.NewLine);
                    html.Append("<div>" + Environment.NewLine);
                    html.Append($"<a href='{categoryArticleListLink}'>");
                    html.Append("<i class='fa fa-angle-double-right'></i> " + model.Name);
                    html.Append("</a>" + Environment.NewLine);
                    html.Append("</div>" + Environment.NewLine);
                    if (model.Children.Count > 0)
                    {
                        html.Append(GetCategoryMenu(model.Id));
                    }

                    html.Append("</li>" + Environment.NewLine);
                }

                html.Append("</ul>");

                return html.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        private static string GetBootstrapCategoryMenu(long parentCategoryId = -1)
        {
            try
            {
                var html = new StringBuilder();
                var linkHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var categoryTree = GetCategories(parentCategoryId, 0, false);

                foreach (CategoryViewModel model in categoryTree)
                {
                    if (model.Children.Count > 0)
                    {
                        html.Append("<li class=\"dropdown-submenu pull-left\">" + Environment.NewLine);
                    }
                    else
                    {
                        html.Append("<li>" + Environment.NewLine);
                    }

                    var categoryListLink = linkHelper.Action("Categories", "Home", new { id = model.SefName });
                    html.Append($"<a href='{categoryListLink}'>{model.Name}</a>");
                    html.Append(Environment.NewLine);
                    if (model.Children.Count > 0)
                    {
                        html.Append("<ul class=\"dropdown-menu\">");
                        html.Append(GetBootstrapCategoryMenu(model.Id));
                        html.Append("</ul>");
                    }

                    html.AppendLine("</li>");
                }

                return html.ToString();
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
                var categoryList = new List<CategoryViewModel>();
                using (var db = new KbVaultContext())
                {
                    var categories = db.Categories.Where(c => c.Parent == parentCategoryId || (parentCategoryId == -1 && c.Parent == null)).ToList();
                    foreach (var cat in categories)
                    {
                        var categoryItem = new CategoryViewModel
                        {
                            Id = cat.Id,
                            Name = cat.Name,
                            SefName = cat.SefName,
                            Icon = string.IsNullOrEmpty(cat.Icon) ? "angle-double-right" : cat.Icon,
                            NameForDroplist = cat.Name.PadLeft(cat.Name.Length + depth, '-'),
                            Children = GetCategories(cat.Id, depth + 2)
                        };
                        categoryList.Add(categoryItem);
                        if (createSingleListForDropdown)
                        {
                            categoryList.AddRange(categoryItem.Children);
                        }
                    }
                }

                return categoryList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
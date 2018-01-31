using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KBVault.Core.MVC.Authorization;
using KBVault.Dal;
using KBVault.Web.Helpers;
using KBVault.Web.Models;
using Resources;
using KbUser = KBVault.Dal.Entities.KbUser;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class AccountController : KbVaultAdminController
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            if (Request.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var kmp = new KBVaultMembershipProvider();
                    if (kmp.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        if (string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }

                        return Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        ModelState.AddModelError("LoginError", ErrorMessages.LoginFailed);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult MyProfile(KbUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new KbVaultContext())
                    {
                        var username = ControllerContext.RequestContext.HttpContext.User.Identity.Name;
                        var usr = db.KbUsers.FirstOrDefault(u => u.UserName == username);
                        if (usr == null)
                        {
                            ModelState.AddModelError("UserNotFound", ErrorMessages.UserNotFound);
                            return View(model);
                        }

                        if (KbVaultAuthHelper.ValidateUser(username, model.OldPassword))
                        {
                            usr.Name = model.Name;
                            usr.LastName = model.LastName;
                            usr.Email = model.Email;
                            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.NewPasswordAgain)
                            {
                                KbVaultAuthHelper.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
                            }

                            db.SaveChanges();
                            ShowOperationMessage(UIResources.UserProfileUpdateSuccessful);
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            ShowOperationMessage(ErrorMessages.WrongPassword);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize]
        public ActionResult MyProfile()
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var username = ControllerContext.RequestContext.HttpContext.User.Identity.Name;
                    var usr = db.KbUsers.FirstOrDefault(u => u.UserName == username);
                    if (usr == null)
                    {
                        throw new ArgumentNullException(ErrorMessages.UserNotFound);
                    }

                    var model = new KbUserViewModel(usr);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Remove(int id = -1)
        {
            var result = new JsonOperationResponse
            {
                Successful = false
            };
            try
            {
                using (var db = new KbVaultContext())
                {
                    db.KbUsers.Remove(db.KbUsers.First(u => u.Id == id));
                    db.SaveChanges();
                    result.Successful = true;
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result.ErrorMessage = ex.Message;
                return Json(result);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UserInfo(KbUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new KbVaultContext())
                    {
                        var usr = db.KbUsers.FirstOrDefault(u => u.Id == model.Id);
                        if (usr == null)
                        {
                            ModelState.AddModelError("UserNotFound", ErrorMessages.UserNotFound);
                            return View(model);
                        }

                        if (KbVaultAuthHelper.ValidateUser(model.UserName, model.OldPassword))
                        {
                            usr.Name = model.Name;
                            usr.LastName = model.LastName;
                            usr.Role = model.Role;
                            usr.Email = model.Email;
                            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.NewPasswordAgain)
                            {
                                KbVaultAuthHelper.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
                            }

                            db.SaveChanges();
                            ShowOperationMessage(UIResources.UserListUserEditSuccessful);
                            return RedirectToAction("Users");
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserInfo(int id = -1)
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var usr = db.KbUsers.FirstOrDefault(u => u.Id == id);
                    if (usr == null)
                    {
                        throw new Exception(ErrorMessages.UserNotFound);
                    }

                    var model = new KbUserViewModel(usr);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ErrorMessages.UserNotFound);
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize(Roles="Admin")]
        public ActionResult Users()
        {
            try
            {
                using (var db = new KbVaultContext())
                {
                    var users = db.KbUsers.OrderBy(u => u.UserName).ToList();
                    return View(users);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        /*
        [AllowAnonymous]
        public void CreateAdmin()
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                KbUser usr = KbVaultAuthHelper.CreateUser("admin", "admin", "admin@kbvault.comx", "admin" ,1);
                usr = db.KbUsers.FirstOrDefault(u => u.Id == usr.Id);
                if (usr != null)
                {
                    usr.LastName = "User";
                    usr.Name = "Admin";
                    db.SaveChanges();
                }
            }
        }
        */
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")]KbUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new KbVaultContext())
                    {
                        var usr = KbVaultAuthHelper.CreateUser(model.UserName, model.OldPassword, model.Email, model.Role, KBVaultHelperFunctions.UserAsKbUser(User).Id);
                        usr = db.KbUsers.FirstOrDefault(u => u.Id == usr.Id);
                        if (usr != null)
                        {
                            usr.LastName = model.LastName;
                            usr.Name = model.Name;
                            db.SaveChanges();
                        }

                        return RedirectToAction("Users");
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                AddGlobalException(ex);
                return RedirectToAction("Index", "Error");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
    }
}

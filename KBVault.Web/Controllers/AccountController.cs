using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KBVault.Core.MVC.Authorization;
using KBVault.Web.Models;
using KBVault.Web.Resources;
using NLog;
using KBVault.Dal;
using System.Collections;

namespace KBVault.Web.Controllers
{       
    [Authorize(Roles="Admin")]
    public class AccountController : KbVaultAdminController
    {
        //
        // GET: /Account/

        //private Logger Log = LogManager.GetCurrentClassLogger();

        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Authorize]
        public ActionResult Logout() 
        { 
            if (Request.IsAuthenticated) 
                FormsAuthentication.SignOut(); 
            return RedirectToAction("Index", "Home"); 
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var kmp = new KBVaultMembershipProvider();
                    if (kmp.ValidateUser(model.UserName, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        if (String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                            return RedirectToAction("Index", "Dashboard");
                        else
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
        public ActionResult MyProfile(KbUserViewModel model)
        {
            try
            {
                                
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        string username = ControllerContext.RequestContext.HttpContext.User.Identity.Name;
                        KbUser usr = db.KbUsers.FirstOrDefault(u => u.UserName == username);
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
                            if (!String.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.NewPasswordAgain)
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

        public ActionResult MyProfile()
        {
            try
            {
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    string username = ControllerContext.RequestContext.HttpContext.User.Identity.Name;
                    KbUser usr = db.KbUsers.FirstOrDefault(u => u.UserName == username);
                    if (usr == null)
                        throw new ArgumentNullException(ErrorMessages.UserNotFound);
                    KbUserViewModel model = new KbUserViewModel(usr);
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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Remove(int id = -1 )
        {
            JsonOperationResponse result = new JsonOperationResponse()
            {
                Successful = false
            };
            try
            {
                using(KbVaultEntities db = new KbVaultEntities())
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
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        KbUser usr = db.KbUsers.FirstOrDefault(u => u.Id == model.Id);
                        if (usr == null)
                        {
                            ModelState.AddModelError("UserNotFound", ErrorMessages.UserNotFound);
                            return View(model);
                        }
                        if( KbVaultAuthHelper.ValidateUser(model.UserName,model.OldPassword) )
                        {
                            usr.Name = model.Name;
                            usr.LastName = model.LastName;
                            usr.Role = model.Role;
                            usr.Email = model.Email;                            
                            if (!String.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.NewPasswordAgain)
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
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    KbUser usr = db.KbUsers.FirstOrDefault(u => u.Id == id);
                    if (usr == null)
                    {
                        throw new Exception(ErrorMessages.UserNotFound);
                    }
                    KbUserViewModel model = new KbUserViewModel(usr);
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
                using (KbVaultEntities db = new KbVaultEntities())
                {
                    List<KbUser> Users = db.KbUsers.OrderBy(u => u.UserName).ToList();
                    return View(Users);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ShowOperationMessage(ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }

        [AllowAnonymous]
        public void CreateAdmin()
        {
            using (KbVaultEntities db = new KbVaultEntities())
            {
                KbUser usr = KbVaultAuthHelper.CreateUser("admin", "admin", "admin@kbvault.comx", "admin");
                usr = db.KbUsers.FirstOrDefault(u => u.Id == usr.Id);
                if (usr != null)
                {
                    usr.LastName = "User";
                    usr.Name = "Admin";
                    db.SaveChanges();
                }
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")]KbUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (KbVaultEntities db = new KbVaultEntities())
                    {
                        KbUser usr = KbVaultAuthHelper.CreateUser(model.UserName, model.OldPassword, model.Email, model.Role);
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
                ShowOperationMessage(ex.Message);
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

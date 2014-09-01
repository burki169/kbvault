using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class KbVaultAdminController : Controller
    {
        protected Logger Log = LogManager.GetCurrentClassLogger();

        private string OperationMessageKey = "KBVAULT_OPERATION_MSG_KEY";
        private string ErrorMessageKey = "KBVAULT_ERROR_MSG_KEY";  
              

        public KbVaultAdminController()
        {            
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.ShowOperationMessage = false;
            if (ControllerContext.HttpContext.Session[OperationMessageKey] != null)
            {
                ViewBag.ShowOperationMessage = true;
                ViewBag.OperationMessage = ControllerContext.HttpContext.Session[OperationMessageKey].ToString();
                ControllerContext.HttpContext.Session.Remove(OperationMessageKey);
            }
            if (ControllerContext.HttpContext.Session[ErrorMessageKey] != null)
            {
                ViewBag.ShowErrorMessage = true;
                ViewBag.ErrorMessage = ControllerContext.HttpContext.Session[ErrorMessageKey].ToString();
                ControllerContext.HttpContext.Session.Remove(ErrorMessageKey);
            }
            ViewBag.ThreadShortDateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            base.OnActionExecuted(filterContext);
        }

        protected void ShowOperationMessage(string msg)
        {
            ControllerContext.HttpContext.Session[OperationMessageKey] += msg;            
        }

        protected void ShowErrorMessage(string msg)
        {
            ControllerContext.HttpContext.Session[ErrorMessageKey] += msg;
        }
        

    }
}

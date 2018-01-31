using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KBVault.Dal;
using KBVault.Web.Models;

namespace KBVault.Web.Controllers
{
    [Authorize]
    public class ActivityController : KbVaultAdminController
    {
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public JsonResult Get(ActivityDataTablesPostModel model)
        {
            try
            {
                var length = model.length;
                var page = model.start / length;
                var result = new JsonOperationResponse();
                using (var db = new KbVaultContext())
                {
                    var recordCount = db.Activities.Count();
                    db.Configuration.LazyLoadingEnabled = false;
                    var activities = db.Activities.Include("KbUser")
                                    .OrderByDescending(a => a.ActivityDate)
                                    .Skip(page * length)
                                    .Take(length).AsEnumerable()
                                    .Select(a => new ActivityViewModel
                                    {
                                        ActivityDate = a.ActivityDate.ToString("dd/MM/yyyy H:mm"),
                                        Operation = a.Operation,
                                        Text = a.Information,
                                        User = a.KbUser.Name + " " + a.KbUser.LastName
                                    }).ToList();
                    result.Successful = true;
                    result.Data = activities;
                    return Json(new { recordsFiltered = recordCount, recordsTotal = recordCount, Successfull = result.Successful, ErrorMessage = result.ErrorMessage, data = ((List<ActivityViewModel>)result.Data).Select(aw => new[] { aw.ActivityDate, aw.Operation, aw.Text, aw.User }) }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}

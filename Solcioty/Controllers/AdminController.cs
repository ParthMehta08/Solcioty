using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using web;

namespace Solcioty.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        [OutputCache(Duration = 300, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        public ActionResult Index()
        {
            //check if admin then redirect to Dashboard
            return RedirectToAction("Dashboard");
        }

        [OutputCache(Duration = 300, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        public ActionResult Dashboard()
        {
            //var userData = Session["UserData"] as UserData;

            //if (userData.RoleCode != RoleTypes.SuperAdmin)
            //{
            //    return RedirectToAction("Unauthorized", "Account");
            //}
            return View();
        }
    }
}
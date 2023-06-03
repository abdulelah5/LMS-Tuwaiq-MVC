using LMS_Project_Week2.CommonClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Project_Week2.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Guest()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        public ActionResult Admin()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        //TODO: When click logout for admin make => System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null
    }
}
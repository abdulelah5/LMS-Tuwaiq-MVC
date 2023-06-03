using LMS_Project_Week2.CommonClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Project_Week2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        public ActionResult About()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
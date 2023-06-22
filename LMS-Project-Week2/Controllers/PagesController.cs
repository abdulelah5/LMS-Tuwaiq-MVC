using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
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
        //This class didn't used also , just Admin redirect without reason actuly *_*

        // GET: Pages
        //public ActionResult Guest()
        //{            
        //    return View();
        //}
        public ActionResult Admin()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Index", "Admin"); ;
        }        
    }
}
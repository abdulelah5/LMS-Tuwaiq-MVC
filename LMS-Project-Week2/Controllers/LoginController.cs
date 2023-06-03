using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
using LMS_Project_Week2.CommonClass;

namespace LMS_Project_Week2.Controllers
{
    public class LoginController : Controller
    {
        public static readonly Crud_Operation Instance = new Crud_Operation();
        // GET: Login
        public ActionResult Login()
        {
            System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if(ModelState.IsValid && !string.IsNullOrWhiteSpace(user.username.Trim()) && !string.IsNullOrWhiteSpace(user.password.Trim()))
            {
               bool isSuccess = Instance.UserCheck(user);
                if (isSuccess)
                {
                    Session["Username"] = user.username;
                    System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] = user.username;
                    return RedirectToAction("Admin", "Pages");
                }
                else
                {
                    ViewBag.Message = "uncorrect username or password";
                    return View(user);
                }
            }
            //ViewBag.Message = "Enter valid username and password";
            return View(user);
        }
    }
}
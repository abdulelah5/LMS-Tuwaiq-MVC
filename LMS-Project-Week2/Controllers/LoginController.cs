using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
using LMS_Project_Week2.CommonClass;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;

namespace LMS_Project_Week2.Controllers
{
    public class LoginController : Controller
    {
        //For biz
        public static readonly Crud_Operation Instance = new Crud_Operation();

        // GET: Login
        #region Login
        public ActionResult Login()
        {            
            System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(user.username.Trim()) && !string.IsNullOrWhiteSpace(user.password.Trim()))
            {
                List<string> role = Instance.UserCheck(user);//We will check for username and password - it will use Upper in SQL so characters in username will be fine
                if (role.Count > 0)//This mean user has account then we will check if its active and which type is it
                {
                    Session["Username"] = user.username.ToLower();
                    Session["Role"] = role.First();
                    Session["userID"] = role.Last();
                    System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] = user.username;
                    if (role.First() == "1")//1 is the Admin
                    {
                        return RedirectToAction("Admin", "Pages");
                    }
                    else
                    {//here we will check for clients 
                        var client = Instance.GetClientById(Convert.ToInt32(role.Last()));
                        if (client.Status.ToUpper() == "A") // Approved can access
                            return RedirectToAction("Index", "Guest");
                        else
                        {
                            Session["Username"] = null;
                            Session["Role"] = null;
                            Session["userID"] = null;
                            System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] = null;
                            if (client.Status == "E") // Email not confirmed can't access
                            {
                                ViewBag.Message = "Please confirm your email";
                                return View(user);
                            }
                            else if (client.Status == "1") // Admin not approved can't access
                            {
                                ViewBag.Message = "Please contact the admin to confirm your account";
                                return View(user);
                            }
                            else if (client.Status.ToUpper() == "D")//blocked can't access
                            {
                                ViewBag.Message = "Your account is blocked";
                                return View(user);
                            }
                        }
                    }
                }
                else //wrong username or password
                {
                    ViewBag.Message = "Incorrect username or password";
                    return View(user);
                }
            }
            //ViewBag.Message = "Enter valid username and password";
            return View(user);
        }
        #endregion

        #region registrition
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Client client)
        {
            if (ModelState.IsValid)//validation check
            {
                //this will check if the username is exist or not because its uniq username
                var isExist = Instance.CheckExistUsername(client.ClientName);
                if (!isExist)
                {
                    bool isSuccess = Instance.RegisterClient(client);
                    if (isSuccess)
                    {
                        //This all faild tries to send email confirmation ^_^
                        #region faild email confirmation tries
                        //               System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                        //new System.Net.Mail.MailAddress("mshoth4@gmail.com", "LMS"),
                        //new System.Net.Mail.MailAddress("mokn4abood@gmail.com", client.ClientName));
                        //               m.Subject = "Email confirmation";
                        //               m.Body = string.Format("Dear {0} " +
                        //               "< BR /> Thank you for your registration, please click on the " +
                        //                  "below link to complete your registration: < a href =\"{1}\" " +
                        //                  "title =\"User Email Confirm\">{1}</a>",
                        //                  client.ClientName, Url.Action("ConfirmEmail", "Account",
                        //                  new { Token = client.Id, Email = client.Email }, Request.Url.Scheme));
                        //               m.IsBodyHtml = true;
                        //               System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                        //               smtp.Credentials = new System.Net.NetworkCredential("mshoth4@gmail.com", "password");

                        //MailMessage mail = new MailMessage();
                        //System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient();
                        //mail.To.Add("mokn4abood@gmail.com");
                        //mail.From = new MailAddress("mshoth4@gmail.com");
                        //mail.Subject = "Email confirmation";
                        //mail.IsBodyHtml = true;
                        //mail.Body = string.Format("Dear {0} " +
                        //               "< BR /> Thank you for your registration, please click on the " +
                        //                  "below link to complete your registration: ",
                        //                  client.ClientName);
                        //SmtpServer.Host = "smtp.gmail.com";
                        //SmtpServer.UseDefaultCredentials = true;
                        //SmtpServer.Credentials = new System.Net.NetworkCredential("mshoth4@gmail.com", "password");
                        //SmtpServer.Port = 587;
                        //SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        //try
                        //{
                        //    SmtpServer.Send(mail);
                        //}
                        //catch (Exception ex)
                        //{
                        //}

                        //var email = new MimeMessage();

                        //email.From.Add(new MailboxAddress("LMS", "coisomk843@exbts.com"));
                        //email.To.Add(new MailboxAddress(client.ClientName, "coisomk843@exbts.com"));

                        //email.Subject = "Testing out email sending";
                        //email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                        //{
                        //    Text = "Hello all the way from the land of C#"
                        //};
                        //using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                        //{
                        //    smtp.Connect("crazymailing.com", 587, false);

                        //    // Note: only needed if the SMTP server requires authentication
                        //    smtp.Authenticate("smtp_username", "smtp_password");

                        //    smtp.Send(email);
                        //    smtp.Disconnect(true);
                        //}

                        //MailMessage mm = new MailMessage("mshoth4@gmail.com", "mokn4abood@gmail.com");
                        //mm.Subject = "LMS Email confirmation";
                        //mm.Body = "Test";
                        //mm.IsBodyHtml = false;

                        //SmtpClient smtp = new SmtpClient();
                        //smtp.Host = "smtp.gmail.com";
                        //smtp.Port = 587;
                        //smtp.EnableSsl = true;

                        //NetworkCredential nc = new NetworkCredential("mshoth4@gmail.com", "password");
                        //smtp.UseDefaultCredentials = true;
                        //smtp.Credentials = new NetworkCredential("mshoth4@gmail.com", "password");
                        //smtp.Send(mm);

                        //WebMail.Send("mokn4abood@gmail.com", "LMS Email confirmation", "Test",
                        //    null, null, null, true, null, null, null, null, null, null);
                        #endregion

                        ViewBag.Message = "Successfuly registerd, Please wait for admin to confirm your account.";
                        return View();
                    }
                }
                else
                {//username exist try again
                    ViewBag.Error = "Try with another username, this one exist";
                    return View(client);
                }

            }
            ViewBag.Error = "Please enter valid data";//validation issue
            return View(client);
        }
        #endregion        
    }
}
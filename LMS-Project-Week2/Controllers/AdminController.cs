using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using PagedList;
using LMS_Project_Week2.CommonClass;
using System.IO;

namespace LMS_Project_Week2.Controllers
{
    public class AdminController : Controller
    {
        //For Biz layer
        public static readonly Crud_Operation operation = Crud_Operation.Instance;

        // GET: Admin
        //Main view for Admin
        public ActionResult Index(int? page, string searchString)
        {
            //if not loged in will redirect to Login page
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //if not admin will redirect to guest home page
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            int pageSize = 5;
            int pageNum;
            if (page == null)
            {
                pageNum = (page ?? 1);
            }
            else
            {
                pageNum = (int)page;
            }

            var books = new List<Book>();
            var client = new Client();
            books = operation.GetList();
            foreach (Book item in books)
            {
                //get author for each book
                item.author = operation.GetAuthor(item.AuthorId);
                if (item.IsAvailable != "Yes")
                {
                    //if not available then we will get the client that have it
                    item.client = operation.GetClient(item.Id);
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.ToLower().Contains(searchString.ToLower())).ToList();
            }

            return View(books.ToList().ToPagedList(pageNum, pageSize));
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            //var books = new List<Book>();
            //books = operation.GetList();
            //foreach (var item in books)
            //{
            //    item.author = operation.GetAuthor(item.AuthorId);
            //    if (item.IsAvailable.ToLower() == "no")
            //        item.client = operation.GetClient(item.Id);
            //}

            var books = operation.GetBookById(id);
            ViewBag.Authors = operation.GetAuthors();
            books.author = operation.GetAuthor(books.AuthorId);
            if (books.IsAvailable.ToLower() == "no")
                books.client = operation.GetClient(books.Id);
            else
                books.client = new Client();

            if (books.LastClient != 0 && books.LastClient != null)
            {
                var temp = operation.GetClientById(Convert.ToInt32(books.LastClient));
                books.LastClientName = temp.ClientName;
            }

            return View(books);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            ViewBag.Authors = operation.GetAuthors();
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(Book book, HttpPostedFileBase file)
        {
            try
            {
                ViewBag.Authors = operation.GetAuthors();
                if (file != null) //if there is file
                {
                    var ext = Path.GetExtension(file.FileName).ToUpper();
                    if (ext == ".PDF") //accept only pdf files !
                    {

                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                        file.SaveAs(_path);

                        Author authorInfo = new Author();
                        authorInfo.Name = book.author.Name;
                        book.AuthorId = book.author.Id;
                        book.Path = _FileName;
                        if (operation.Add_Book(book, authorInfo, Session["Username"].ToString()))
                        {
                            ViewBag.Message = "Book Details Added Successfully";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "PDF file only!";

                    }
                }
                else if (file == null) //if there is no pdf file
                {
                    Author authorInfo = new Author();
                    authorInfo.Name = book.author.Name;
                    book.AuthorId = book.author.Id;
                    book.Path = "";
                    if (operation.Add_Book(book, authorInfo, Session["Username"].ToString()))
                    {
                        ViewBag.Message = "Book Details Added Successfully";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                }


                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            var books = operation.GetList();
            ViewBag.Authors = operation.GetAuthors();
            foreach (var item in books)
            {
                item.author = operation.GetAuthor(item.AuthorId);
                if (item.IsAvailable.ToLower() == "no")
                    item.client = operation.GetClient(item.Id);
                else
                    ViewBag.client = operation.GetFreeClient();
            }
            return View(books.Find(x => x.Id == id));
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Book book, HttpPostedFileBase file, string Remove)
        {
            try
            {
                ViewBag.Authors = operation.GetAuthors();
                book.Id = id;
                book.LastUpdateBy = Session["Username"].ToString();
                book.LastUpdateDT = DateTime.Now.ToString();
                //if (book.IsAvailable.ToLower() == "yes" && book.client.Id != 0 && book.client.Id != null)
                //{
                //    book.IsAvailable = operation.UpdateClientBookId(book) ? "No" : "Yes";
                //}

                if (file != null) //if the admin want to update the existing pdf file
                {
                    var ext = Path.GetExtension(file.FileName).ToUpper();
                    if (ext == ".PDF") //must be pdf !
                    {

                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                        file.SaveAs(_path);

                        book.Path = _FileName;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "PDF file only!";
                        //return RedirectToAction("Edit",book.Id);
                        return View(book);
                    }
                }
                else if (file == null) // if the admin want to update details not the exist file
                {
                    if (string.IsNullOrWhiteSpace(Remove))//if the admin don't want to remove the existing pdf only update data
                        book.Path = "";
                    else
                        book.Path = Remove; //if the admin want to remove the pdf file and it can update book data in same time
                }

                if (operation.UpdateBook(book, Session["Username"].ToString()))//Will store user that updated 
                {
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            var books = operation.GetList();
            foreach (var item in books)
            {
                item.author = operation.GetAuthor(item.AuthorId);
            }
            return View(books.Find(x => x.Id == id));
        }

        // POST: Admin/Delete/5        
        [HttpPost]
        public ActionResult Delete(int id, Book book)
        {
            try
            {
                book.Id = id;
                book.LastUpdateBy = Session["Username"].ToString();
                book.LastUpdateDT = DateTime.Now.ToString();
                if (operation.DeleteBook(book))
                {
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Users(int? page)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            int pageSize = 5;
            int pageNum;
            if (page == null)
            {
                pageNum = (page ?? 1);
            }
            else
            {
                pageNum = (int)page;

            }
            var users = new List<Client>();
            users = operation.GetUsersList();

            return View(users.ToList().ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        public ActionResult ApproveUser(int id)
        {
            try
            {
                var user = new Client();
                user.Id = id;
                user.Status = "A";  //A means Approved and 1 not approved and D is deleted or rejected
                user.LastUpdateBy = Session["Username"].ToString();
                user.LastUpdateDT = DateTime.Now.ToString();
                if (operation.UpdateUserStatus(user))
                {
                    return RedirectToAction("Users");
                }
                ViewBag.Error = "User not updated, something went wrong";
                return RedirectToAction("Users");
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        public ActionResult RejectDeleteUser(int id)
        {

            try
            {
                var user = new Client();
                user.Id = id;
                user = operation.GetClientById(id);
                user.LastUpdateBy = Session["Username"].ToString();
                user.LastUpdateDT = DateTime.Now.ToString();
                if (operation.DeleteUser(user))
                {
                    //update book availability for this client which has been deleted
                    if (user.BookId != 0 || user.BookId != null)
                        operation.ReturnBook(user.BookId);
                    return RedirectToAction("Users");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult BookHistory(int id, int? page, string searchString)
        {
            //if not loged in will redirect to Login page
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //if not admin will redirect to guest home page
            if (Session["Role"].ToString() != "1")
                return RedirectToAction("Index", "Guest");

            int pageSize = 5;
            int pageNum;
            if (page == null)
            {
                pageNum = (page ?? 1);
            }
            else
            {
                pageNum = (int)page;
            }

            var books = new List<Book>();
            var client = new Client();
            books = operation.SelectHBook(id);
            foreach (Book item in books)
            {
                item.Title = operation.GetBookById(item.Id).Title;
                //get author for each book
                item.author = operation.GetAuthor(item.AuthorId);
                if (item.IsAvailable != "Yes")
                {
                    //if not available then we will get the client that have it
                    item.client = operation.GetClientById(Convert.ToInt32(item.client.Id));
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.client.ClientName.ToLower().Contains(searchString.ToLower())).ToList();
            }

            return View(books.ToList().ToPagedList(pageNum, pageSize));
        }
    }
}

using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using LMS_Project_Week2.CommonClass;

namespace LMS_Project_Week2.Controllers
{
    public class AdminController : Controller
    {
        public static readonly Crud_Operation operation = Crud_Operation.Instance;
        // GET: Admin
        public ActionResult Index(int? page, string searchString)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }

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
                item.author = operation.GetAuthor(item.AuthorId);
                if (item.IsAvailable != "Yes")
                {
                    item.client = operation.GetClient(item.Id);
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                books = (List<Book>)books.Where(b => b.Title.Contains(searchString)).ToList();
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

            var books = new List<Book>();
            books = operation.GetList();
            foreach (var item in books)
            {
                item.author = operation.GetAuthor(item.AuthorId);
                if (item.IsAvailable.ToLower() == "no")
                    item.client = operation.GetClient(item.Id);
            }
            return View(books.Find(x => x.Id == id));
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Authors = operation.GetAuthors();            
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(Book book)
        {
            try
            {
                //if (ModelState.IsValid)
                {
                    Author authorInfo = new Author();
                    authorInfo.Name = book.author.Name;
                    book.AuthorId = book.author.Id;
                    if (operation.Add_Book(book, authorInfo))
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
        public ActionResult Edit(int id, Book book)
        {
            try
            {
                book.Id = id;
                if(book.IsAvailable.ToLower() == "yes" && book.client.Id != 0 && book.client.Id != null)
                {
                    book.IsAvailable = operation.UpdateClientBookId(book) ? "No" : "Yes";
                }

                if (operation.UpdateBook(book))
                {
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch(Exception ex)
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
            var books = operation.GetList();
            foreach(var item in books)
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
    }
}

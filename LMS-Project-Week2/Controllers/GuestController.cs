using LMS_Project_Week2.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Project_Week2.Controllers
{
    public class GuestController : Controller
    {
        private LibraryTwaiqWeek1Entities db = new LibraryTwaiqWeek1Entities();

        // GET: Guest
        public ActionResult Index(int? page, string searchString)
        {
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


            var result = db.Books.OrderBy(x => x.AuthorId);

            if (!String.IsNullOrEmpty(searchString))
            {
                result = (IOrderedQueryable<Books>)result.Where(b => b.Title.Contains(searchString));
            }



            return View(result.ToList().ToPagedList(pageNum, pageSize));
        }

        // GET: Guest/Details/5
        public ActionResult Rent(int id)
        {
            var book = db.Books.Find(id);
            ViewBag.book = book;
            if (book.IsAvailable.ToUpper().Equals("NO"))
            {
                ViewBag.errorMessage = "the book is is not available ";
            }
            return View();
        }


        [HttpPost]
        public ActionResult Rent(int id, string name)
        {

            var client = new Client();
            client.BookId = id;
            client.ClientName = name;
            db.Client.Add(client);

            var book = db.Books.Find(id);
            book.IsAvailable = "NO";
            db.SaveChanges();


            ViewBag.book = book;
            ViewBag.okMessage = "thank you for rent our book";


            return View();
        }

        public ActionResult ReturnBook()
        {
            var books = db.Books.Where(b => b.IsAvailable.ToUpper().Equals("NO"));

            return View(books);
        }
        [HttpPost]
        public ActionResult ReturnBook(int id)
        {
            var book = db.Books.Find(id);
            book.IsAvailable = "YES";
            db.SaveChanges();

            var books = db.Books.Where(b => b.IsAvailable.ToUpper().Equals("NO"));
            ViewBag.okMessage = "thank you for return our book";
            return View(books);
        }


    }
}

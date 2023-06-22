using LaibrarySystem.Biz;
using LMS_Project_Week2.CommonClass;
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
        public static readonly Crud_Operation operation = Crud_Operation.Instance;
        // GET: Guest
        //Home page for clients
        public ActionResult Index(int? page, string searchString)
        {
            if (Session["Role"] == null) //If role null thats meanning is a guest not client which will be only able to see not to use any service
                Session["Role"] = "0";

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


            var result = db.Books.AsQueryable();// .OrderBy(x => x.AuthorId);
            //var authors = db.Author.AsQueryable();
            foreach (var item in result)
            {
                Author author = db.Author.Find((int)item.AuthorId);
                item.Author = author;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                result = (IOrderedQueryable<Books>)result.Where(b => b.Title.Contains(searchString));
            }


            return View(result.ToList().ToPagedList(pageNum, pageSize));
        }

        // GET: Guest/Details/5
        public ActionResult Rent(int id)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Role"].ToString() != "2") 
            {
                //Role = 2 ? this mean there is user logged in but its not client its with another type like admin so he will be not able to use the services as its for clients only
                return RedirectToAction("Index");
            }
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
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //var client = new Client();
            //client.BookId = id;
            //client.ClientName = name;
            //db.Client.Add(client);

            var clientId = Convert.ToInt32(Session["userID"].ToString());
            var client = db.Client.SingleOrDefault(x => x.ClientId == clientId);
            if(client.BookId != 0 && client.BookId != null)
            {//if the client already rent a book that he didn't return it back he will be not able to rent another book until return that book he have its
                ViewBag.No = "Please return your book first";
                return View();
            }   
            
            client.BookId = id;

            var book = db.Books.Find(id);
            book.IsAvailable = "NO";
            book.RentDate = DateTime.Now.ToString();
            db.SaveChanges();


            ViewBag.book = book;
            ViewBag.okMessage = "Thank you for rent our book";//success


            return View();
        }

        public ActionResult ReturnBook()
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if(Session["Role"].ToString() != "2")//this service for client only so others can't use it
            {
                return RedirectToAction("Index");
            }
            var clientId = Convert.ToInt32(Session["userID"].ToString());
            var client = db.Client.Where(x => x.ClientId == clientId).ToList();
            //var books = db.Books.Where(b => b.IsAvailable.ToUpper().Equals("NO"));
            var BookId = client.First().BookId;
            var books = db.Books.Where(b => b.Id == BookId);

            return View(books);
        }
        [HttpPost]
        public ActionResult ReturnBook(int id)
        {
            if (System.Web.HttpContext.Current.Session[AuthinticationLoginSession.loggedInUserObject] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var book = db.Books.Find(id);
            book.IsAvailable = "YES";
            book.ReturnDate = DateTime.Now.ToString();            

            var clientId = Convert.ToInt32(Session["userID"].ToString());
            book.LastClientId = clientId;            

            var client = db.Client.Where(x => x.ClientId == clientId).FirstOrDefault();
            client.BookId = 0;

            //Save Book History
            LaibrarySystem.Dto.Book book1 = operation.GetBookById(id);
            book1.client = new LaibrarySystem.Dto.Client();
            book1.client.Id = clientId;
            book1.ReturnDate = DateTime.Now.ToString();
            bool isSuccess = operation.UpdateHBooks(book1);

            db.SaveChanges();            

            //var books = db.Books.Where(b => b.IsAvailable.ToUpper().Equals("NO"));
            var BookId = client.BookId;
            var books = db.Books.Where(b => b.Id == BookId);            

            ViewBag.okMessage = "Thank you for return our book";
            return View(books);
        }

        //To open pdf preview for requested book
        public ActionResult PdfViewer(int id)
        {
            var book = operation.GetList().Find(x => x.Id == id);

            var request = System.Web.HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var pdfUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl) + "UploadedFiles/" + book.Path;



            return Redirect(pdfUrl);
        }

    }
}

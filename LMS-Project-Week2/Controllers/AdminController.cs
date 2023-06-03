using LaibrarySystem.Biz;
using LaibrarySystem.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS_Project_Week2.Controllers
{
    public class AdminController : Controller
    {
        public static readonly Crud_Operation operation = Crud_Operation.Instance;
        // GET: Admin
        public ActionResult Index()
        {
            var books = new List<Book>();
            books = operation.GetList();
            return View(books);
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            var books = new List<Book>();
            books = operation.GetList();
            return View(books.Find(x => x.Id == id));
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(Book book, string author)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Author authorInfo = new Author();
                    authorInfo.Name = author;
                    if(operation.Add_Book(book, authorInfo))
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
            var books = operation.GetList();
            return View(books.Find(x => x.Id == id));
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Book book)
        {
            try
            {
                book.Id = id;
                if (operation.UpdateBook(book))
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

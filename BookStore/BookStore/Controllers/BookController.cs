using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Models.Repositories;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookstoreRepository<Book> bookRepository;
        private readonly IBookstoreRepository<Author> authorRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public BookController(IBookstoreRepository<Book> bookRepository, IBookstoreRepository<Author> authorRepository, IHostingEnvironment hostingEnvironment )
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        // GET: Book
        public ActionResult Index()
        {
            var books = bookRepository.List();
            return View(books);
        }

        // GET: Book/Details/5
        public ActionResult Details(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            var book = new BookAuthorViewModel()
            {
                Authors = FillAuthorsSelect()
            };
            return View(book);
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    string fileName = UploadFile(model.File) ?? string.Empty;

                    if (model.AuthorId == -1)
                    {
                        ViewBag.Message = "Please select an Author from list";
                        model.Authors = FillAuthorsSelect();
                        return View(model);
                    }

                    var author = authorRepository.Find(model.AuthorId);
                    Book book = new Book
                    {
                        Id = model.BookId,
                        Title = model.Title,
                        Description = model.Description,
                        Author = author,
                        ImageUrl = fileName

                    };
                    bookRepository.Add(book);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

            ModelState.AddModelError("", "You have to fill all required fields !!!");
            model.Authors = FillAuthorsSelect();
            return View(model);
            
        }

        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            var book = bookRepository.Find(id);
            var authorId = book.Author == null ? book.Author.Id = 0 : book.Author.Id;

            var viewNodel = new BookAuthorViewModel
            {
                BookId = book.Id,
                Description = book.Description,
                Title = book.Title,
                AuthorId = authorId,
                Authors = authorRepository.List().ToList(),
                ImageUrl = book.ImageUrl

            };

            return View(viewNodel);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookAuthorViewModel viewModel)
        {
            try
            {
                // TODO: Add update logic here

                string fileName = UploadFile(viewModel.File, viewModel.ImageUrl);

                var author = authorRepository.Find(viewModel.AuthorId);
                Book book = new Book
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Author = author,
                    ImageUrl = fileName

                };
                bookRepository.Update(viewModel.BookId, book);


                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                bookRepository.Delete(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        List<Author> FillAuthorsSelect()
        {
            List<Author> authors = authorRepository.List().ToList();

            authors.Insert(0, new Author { Id = -1, FullName = "--- Please select an author ---" });

            return authors;
        }


        BookAuthorViewModel GetAllAuthors()
        {
            var vmodel = new BookAuthorViewModel
            {
                Authors = FillAuthorsSelect()
            };
            return vmodel;
        }


        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                string fullPath = Path.Combine(uploads, file.FileName);
                //save Image

                FileStream fs = new FileStream(fullPath, FileMode.Create);

                file.CopyTo(fs);

                fs.Close();
                fs.Dispose();

                return file.FileName;

            }

            return null;
        }

        string UploadFile(IFormFile file, string imageUrl)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                string newPath = Path.Combine(uploads, file.FileName);
                //delete old Image

                string fileOldPath = Path.Combine(uploads, imageUrl);
                if (newPath != fileOldPath)
                {
                    System.IO.File.Delete(fileOldPath);
                    //save Image
                    FileStream fs = new FileStream(newPath, FileMode.Create);
                    file.CopyTo(fs);

                    fs.Close();
                    fs.Dispose();
                }

                return file.FileName;
            }

            return imageUrl;
        }
    }
}
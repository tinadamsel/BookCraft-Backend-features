using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using static Core.DB.BooksphereEnums;

namespace Bookcraft.Controllers
{
    public class BookController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IDropdownHelper _dropdownHelper;
        private readonly AppDbContext _context;
        private readonly IBookService _bookService;
        private readonly ICoverGenerationService _coverService;
        private readonly IWebHostEnvironment _env;

        public BookController(IUserHelper userHelper, AppDbContext context, IBookService bookService, ICoverGenerationService coverService, IDropdownHelper dropdownHelper, IWebHostEnvironment env)
        {
            _userHelper = userHelper;
            _context = context;
            _bookService = bookService;
            _coverService = coverService;
            _dropdownHelper = dropdownHelper;
            _env = env;
        }

        [HttpGet]
        public IActionResult CreateNewBook()
        {
            ViewBag.Genre = _dropdownHelper.DropdownOfGenres();
            ViewBag.TargetAudience = _dropdownHelper.DropdownOfAudience();
            ViewBag.WritingStyles = _dropdownHelper.DropdownOfWritingStyles();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateNewBook(string bookDetails)
        {
            if (bookDetails != null)
            {
                var bookViewModel = JsonConvert.DeserializeObject<BookViewModel>(bookDetails);
                if (bookViewModel != null)
                {
                    if (bookViewModel.BookTitle == "" || bookViewModel.BookTitle == null)
                    {
                        return Json(new { isError = true, msg = "Please fill in your book name" });
                    }
                    var currentUser = _userHelper.FindByUserName(User.Identity.Name);
                    if (currentUser == null)
                    {
                        return Json(new { isError = true, msg = "User Not Loggedin" });
                    }
                    var saveNewBookDetails = await _bookService.SaveBookDetailsToDbAsync(bookViewModel, currentUser);
                    if (saveNewBookDetails != null)
                    {
                      var generateBook = await _bookService.GenerateBookChaptersAsync(saveNewBookDetails.Id);
                      if (generateBook > 0)
                      {
                         return Json(new { isError = false, bookId = generateBook, msg = "Book Generated Successfully" });
                      }
                    }
                    return Json(new { isError = true, msg = "Unable to Generate" });
                }
            }
            return Json(new { isError = true, msg = "Network Failure" });
        }

        public IActionResult GetBook(int bookId)
        {
            if (bookId > 0)
            {
                var model = _bookService.GetBookWithChapters(bookId);
                if (model != null)
                {
                    return View(model);
                }
            }
            return RedirectToAction("CreateNewBook", "Book");
        }

        [HttpPost]
        public async Task<IActionResult> SaveChapter([FromBody] BookChaptersViewModel model)
        {
            var chapter = await _context.Chapters
                .FirstOrDefaultAsync(c =>
                    c.Id == model.Id &&
                    c.BookId == model.BookId);

            if (chapter == null)
                return BadRequest();

            chapter.Content = model.Content;
            chapter.Status = ChapterStatus.Edited;
            await _context.SaveChangesAsync();

            return Ok(new { isError = false, msg = "Chapter Saved" });
        }

        //download pdf full book 
        //public async Task<IActionResult> DownloadPdf(int bookId)
        //{
        //    try
        //    {
        //        if (bookId > 0)
        //        {
        //            var pdfBytes = await _bookService.ExportBookToPdfAsync(bookId);
        //            return File(pdfBytes, "application/pdf", $"Book_{bookId}.pdf");
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int bookId)
        {
            if (bookId <= 0)
                return BadRequest("Invalid book id.");

            try
            {
                var pdfBytes =  _bookService.ExportBookToPdf(bookId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return NotFound("PDF could not be generated.");

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Book_{bookId}.pdf"
                );
            }
            catch (Exception)
            {
                // Log error here if you want
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }


        //download Doc full book 
        public async Task<IActionResult> DownloadDocx(int bookId)
        {
            var bytes = await _bookService.ExportBookToDocxAsync(bookId);

            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"Book_{bookId}.docx");
        }

        //download epub full book
        public IActionResult DownloadEpub(int bookId)
        {
            var bytes = _bookService.ExportBookToEpub(bookId);

            return File( bytes,"application/epub+zip", $"Book_{bookId}.epub");
        }


        [HttpGet]
        public IActionResult CreateBookCover(int bookId)
        {
            if (bookId > 0)
            {
                var model = new BookViewModel()
                {
                    Id = bookId,
                };
                return View(model);
            }
            return RedirectToAction("CreateNewBook", "Book");
        }


        [HttpPost]

        public async Task<JsonResult> GenerateBookCover(int bookId)
        {
            try
            {
                if (bookId <= 0)
                    return Json(new { isError = true, msg = "Invalid book Id" });

                var book = _bookService.GetBook(bookId);
                if (book == null)
                    return Json(new { isError = true, msg = "Could not find book" });

                var prompt = BuildCoverPrompt(book);
                var imageBytes = await _bookService.GenerateBookCoverAsync(prompt);

                // ✅ Ensure directory exists
                var coversDirectory = Path.Combine(_env.WebRootPath, "book-covers");
                if (!Directory.Exists(coversDirectory))
                {
                    Directory.CreateDirectory(coversDirectory);
                }

                var fileName = $"book-cover-{bookId}.png";
                var filePath = Path.Combine(coversDirectory, fileName);

                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                var coverUrl = $"/book-covers/{fileName}";
                await _bookService.SaveBookCover(bookId, coverUrl);

                return Json(new { isError = false, bookId, coverUrl, msg = "Cover Generated"});
            }
            catch (Exception ex)
            {
                // Optional: log ex here
                return Json(new { isError = true, msg = "An error occurred while generating cover" });
            }
        }
        private string BuildCoverPrompt(Book book)
        {
            return $"""
                A professional book cover design.
                Title: "{book.BookTitle}"
                Genre: {book.Genre?.Name}
                Author: {book.BookAuthor}
                Style: modern, cinematic, high-quality illustration.
                No text except the book title and Author
                Beautiful background and design, dramatic lighting. No watermark, No logo.
                High resolution, clean composition.
                Suitable for an AI-generated book cover.
                """;
        }

        //page that displays the cover
        public IActionResult DisplayBookCover(int bookId)
        {
            if (bookId > 0)
            {
                var book = _bookService.GetBook(bookId);
                if (book != null)
                {
                    var model = new BookViewModel()
                    {
                        Id = bookId,
                        CoverDesignUrl = book.CoverDesignUrl,
                    };
                    return View(model);
                }
                
            }
            return RedirectToAction("CreateNewBook", "Book");
        }

        //to be used when its time
        [HttpGet]
        public IActionResult UserBooks()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity.Name);
            if (currentUser != null)
            {
                var getAllUserBooks = _bookService.GetUserBooks(currentUser);
                if (getAllUserBooks != null)
                {
                    return View(getAllUserBooks);
                }
            }
            return RedirectToAction("Index", "Home");
        }

    }
}

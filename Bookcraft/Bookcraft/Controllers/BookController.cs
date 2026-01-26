using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Bookcraft.Controllers
{
    public class BookController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IDropdownHelper _dropdownHelper;
        private readonly AppDbContext _context;
        private readonly IBookService _bookService;
        private readonly ICoverGenerationService _coverService;

        public BookController(IUserHelper userHelper, AppDbContext context, IBookService bookService, ICoverGenerationService coverService, IDropdownHelper dropdownHelper)
        {
            _userHelper = userHelper;
            _context = context;
            _bookService = bookService;
            _coverService = coverService;
            _dropdownHelper = dropdownHelper;
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
                    var saveNewBookDetails = _bookService.SaveBookDetailsToDbAsync(bookViewModel, currentUser.Id);
                    if (saveNewBookDetails != null)
                    {
                         await _bookService.GenerateBookChaptersAsync(saveNewBookDetails.Id);
                         return Json(new { isError = false, msg = "Book Generated Successfully" });
                        //if (generateBookChapters.Message != "Successful")
                        //{
                        //    return Json(new { isError = true, msg = generateBookChapters.Message });
                        //}
                        //return Json(new { isError = false, msg = "Book Created Successfully" });
                    }
                    return Json(new { isError = true, msg = "Unable to Generate" });
                }
            }
            return Json(new { isError = true, msg = "Network Failure" });
        }


        [HttpGet]
        public IActionResult CreateBookCover()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> GenerateCover(int bookId)
        //{
        //    var imageUrl = await _coverService.GenerateCoverAsync(bookId);
        //    return Ok(imageUrl);
        //}

        //to be used when its time
       // [HttpGet]
        //public IActionResult UserBooks()
        //{
        //    var currentUser = _userHelper.FindByUserName(User.Identity.Name);
        //    if (currentUser != null)
        //    {
        //        var getAllUserBooks = _userHelper.GetUserBooks(currentUser);
        //        if (getAllUserBooks != null)
        //        {
        //            return View(getAllUserBooks);
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

    }
}

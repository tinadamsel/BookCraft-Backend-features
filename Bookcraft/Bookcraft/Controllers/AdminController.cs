using Core.DB;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bookcraft.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext _context;

        public AdminController(IUserHelper userHelper, AppDbContext context)
        {
            _userHelper = userHelper;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult BookGenre()
        {
            var genreDetails = _userHelper.GetGenre();
            return View(genreDetails);
        }
        [HttpPost]
        public JsonResult CreateGenre(string name)
        {
            if (name != null)
            {
                var checkgenreName = _userHelper.CheckExistingGenreName(name);
                if (!checkgenreName)
                {
                    var createGenre = _userHelper.CreateGenre(name);
                    if (createGenre)
                    {
                        return Json(new { isError = false, msg = "Genre Created Successfully" });
                    }
                    return Json(new { isError = true, msg = "Unable to Create" });
                }
                return Json(new { isError = true, msg = "Genre Name Already Created" });
                
            }
            return Json(new { isError = true, msg = "Network Failure" });
        }

        [HttpGet]
        public JsonResult EditGenre(int Id)
        {
            if (Id > 0)
            {
                var genreToEdit = _userHelper.GetGenreToEdit(Id);
                if (genreToEdit != null)
                {
                    return Json(genreToEdit);
                }
                return Json(new { isError = true, msg = "Unable To Get Genre" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        [HttpPost]
        public JsonResult EditedGenre(int id, string name)
        {
            if (id > 0 && name != null)
            {
                var editGenre = _userHelper.SaveEditedGenre(id, name);
                if (editGenre)
                {
                    return Json(new { isError = false, msg = "Genre Edited Successfully" });
                }
                return Json(new { isError = true, msg = "Unable to Edit Genre" });
                
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        public JsonResult DeleteGenre(int id)
        {
            if (id > 0)
            {
                var deleteGenre = _userHelper.DeleteGenre(id);
                if (deleteGenre)
                {
                    return Json(new { isError = false, msg = "Genre Deleted successfully" });
                }
                return Json(new { isError = true, msg = "Unable To Delete Genre" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpGet]
        public IActionResult Audience()
        {
            var audienceDetails = _userHelper.GetAllAudience();
            return View(audienceDetails);
        }

        [HttpPost]
        public JsonResult CreateTargetAudience(string name)
        {
            if (name != null)
            {
                var checkExistingAudience = _userHelper.CheckExistingTargetAudience(name);
                if (!checkExistingAudience)
                {
                    var createTargetAudience = _userHelper.CreateAudience(name);
                    if (createTargetAudience)
                    {
                        return Json(new { isError = false, msg = "Audience Created Successfully" });
                    }
                    return Json(new { isError = true, msg = "Unable to Create" });
                }
                return Json(new { isError = true, msg = "Audience Name Already Created" });

            }
            return Json(new { isError = true, msg = "Network Failure" });
        }

        [HttpGet]
        public JsonResult EditAudience(int Id)
        {
            if (Id > 0)
            {
                var AudienceToEdit = _userHelper.GetAudienceToEdit(Id);
                if (AudienceToEdit != null)
                {
                    return Json(AudienceToEdit);
                }
                return Json(new { isError = true, msg = "Unable To Get Audience" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        [HttpPost]
        public JsonResult EditedAudience(int id, string name)
        {
            if (id > 0 && name != null)
            {
                var editAudience = _userHelper.SaveEditedAudience(id, name);
                if (editAudience)
                {
                    return Json(new { isError = false, msg = "Audience Edited Successfully" });
                }
                return Json(new { isError = true, msg = "Unable to Edit" });

            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        public JsonResult DeleteAudience(int id)
        {
            if (id > 0)
            {
                var deleteAudience = _userHelper.DeleteAudience(id);
                if (deleteAudience)
                {
                    return Json(new { isError = false, msg = "Audience Deleted successfully" });
                }
                return Json(new { isError = true, msg = "Unable To Delete" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpGet]
        public IActionResult WritingStyles()
        {
            var writingstyles = _userHelper.GetWritingStyles();
            return View(writingstyles);
        }

        [HttpPost]
        public JsonResult CreateStyle(string name)
        {
            if (name != null)
            {
                var checkExistingStyle = _userHelper.CheckExistingStyleName(name);
                if (!checkExistingStyle)
                {
                    var createStyle = _userHelper.CreateWritingStyle(name);
                    if (createStyle)
                    {
                        return Json(new { isError = false, msg = "Writing Style Created Successfully" });
                    }
                    return Json(new { isError = true, msg = "Unable to Create" });
                }
                return Json(new { isError = true, msg = "Style Name Already Created" });

            }
            return Json(new { isError = true, msg = "Network Failure" });
        }

        [HttpGet]
        public JsonResult EditStyle(int Id)
        {
            if (Id > 0)
            {
                var styleToEdit = _userHelper.GetStyleToEdit(Id);
                if (styleToEdit != null)
                {
                    return Json(styleToEdit);
                }
                return Json(new { isError = true, msg = "Unable To Get Style" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        [HttpPost]
        public JsonResult EditedStyle(int id, string name)
        {
            if (id > 0 && name != null)
            {
                var editStyle = _userHelper.SaveEditedStyle(id, name);
                if (editStyle)
                {
                    return Json(new { isError = false, msg = "Style Edited Successfully" });
                }
                return Json(new { isError = true, msg = "Unable to Edit" });

            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        public JsonResult DeleteStyle(int id)
        {
            if (id > 0)
            {
                var deleteStyle = _userHelper.DeleteStyle(id);
                if (deleteStyle)
                {
                    return Json(new { isError = false, msg = "Style Deleted successfully" });
                }
                return Json(new { isError = true, msg = "Unable To Delete" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }


    }
}

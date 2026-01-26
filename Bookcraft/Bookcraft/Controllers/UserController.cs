using Core.DB;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Bookcraft.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext _context;
        public UserController(IUserHelper userHelper, AppDbContext context)
        {
            _userHelper = userHelper;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        

    }
}

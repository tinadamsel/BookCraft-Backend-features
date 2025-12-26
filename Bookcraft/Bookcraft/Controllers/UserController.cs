using Microsoft.AspNetCore.Mvc;

namespace Bookcraft.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

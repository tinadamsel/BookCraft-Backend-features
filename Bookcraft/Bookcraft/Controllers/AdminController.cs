using Microsoft.AspNetCore.Mvc;

namespace Bookcraft.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

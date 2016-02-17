using Microsoft.AspNet.Mvc;

namespace Accursed.Controllers
{
    public class HomeController : Controller
    {
        [RouteAttribute("")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Status(int id)
        {
            return View(id);
        }
    }
}

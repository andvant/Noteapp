using Microsoft.AspNetCore.Mvc;

namespace Noteapp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

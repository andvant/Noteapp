using Microsoft.AspNetCore.Mvc;

namespace Noteapp.Web.Controllers
{
    public class AppController : Controller
    {
        public ActionResult Index()
        {
            return File("~/index.html", "text/html");
        }
    }
}

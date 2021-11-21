using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Infrastructure.Data;

namespace Web.Controllers
{
    public class AppController : Controller
    {
        public ActionResult Index()
        {
            return File("~/index.html", "text/html");
        }
    }
}

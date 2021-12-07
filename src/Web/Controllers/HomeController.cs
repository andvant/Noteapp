using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Noteapp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("apiClient");
        }

        [HttpGet("/")]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet("p/{url}")]
        public async Task<IActionResult> ReadPublishedNote(string url)
        {
            var response = await _httpClient.GetAsync($"p/{url}");

            if (response.IsSuccessStatusCode)
            {
                string text = await response.Content.ReadAsStringAsync();
                return View((object)text);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("app")]
        public IActionResult App()
        {
            return File("~/app.html", "text/html");
        }

        [HttpGet("download")]
        public IActionResult DownloadForDesktop()
        {
            string virtualPath = "~/installer/Noteapp-win64-setup.exe";
            string contentType = "application/vnd.microsoft.portable-executable";
            string downloadName = "Noteapp-win64-setup.exe";

            // will crash because there are no .exe files in the git repository
            return File(virtualPath, contentType, downloadName);
        }
    }
}

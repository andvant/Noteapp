using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Threading.Tasks;

namespace Noteapp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;
        public HomeController(IHttpClientFactory factory, IWebHostEnvironment env)
        {
            _httpClient = factory.CreateClient("apiClient");
            _env = env;
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
            return File("app.html", "text/html");
        }

        [HttpGet("download")]
        public IActionResult DownloadAppForDesktop()
        {
            string virtualPath = "download/Noteapp-win64-setup.exe";
            string contentType = "application/vnd.microsoft.portable-executable";
            string downloadName = "Noteapp-win64-setup.exe";

            if (_env.WebRootFileProvider.GetFileInfo(virtualPath).Exists)
            {
                return File(virtualPath, contentType, downloadName);
            }
            else
            {
                return NotFound(new { error = "There's no installer in the git repository" });
            }
        }
    }
}

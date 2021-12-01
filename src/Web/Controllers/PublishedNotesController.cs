using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Noteapp.Web.Controllers
{
    [Route("p")]
    public class PublishedNotesController : Controller
    {
        [HttpGet("{url}")]
        public async Task<IActionResult> Read(string url)
        {
            var client = new HttpClient();

            var response = await client.GetAsync($"http://localhost:5000/p/{url}");

            IEnumerable<string> lines;

            if (response.IsSuccessStatusCode)
            {
                lines = (await response.Content.ReadAsStringAsync()).Split(Environment.NewLine);
            }
            else
            {
                return NotFound();
            }

            return View(lines);
        }
    }
}

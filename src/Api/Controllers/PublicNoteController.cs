using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Data;
using Noteapp.Api.Services;

namespace Noteapp.Api.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class PublicNoteController : ControllerBase
    {
        // hardcode user id for now
        private const int _userId = 1;
        private readonly PublicNoteService _publicNoteService;

        public PublicNoteController(PublicNoteService publicNoteService)
        {
            _publicNoteService = publicNoteService;
        }

        [HttpGet("{url}")]
        public IActionResult Get(string url)
        {
            return Ok(_publicNoteService.GetNoteText(url));
        }

        [HttpPost]
        public IActionResult Publish(int noteId)
        {
            string url = _publicNoteService.PublishNote(_userId, noteId);

            return url != null ? Ok(url) : BadRequest("Invalid noteId");
        }
    }
}

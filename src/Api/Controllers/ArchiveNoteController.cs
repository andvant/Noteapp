using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Data;
using Noteapp.Api.Services;

namespace Noteapp.Api.Controllers
{
    [Route("api/archive")]
    [ApiController]
    public class ArchiveNoteController : ControllerBase
    {
        // hardcode user id for now
        private const int _userId = 1;
        private readonly ArchiveNoteService _archiveNoteService;

        public ArchiveNoteController(ArchiveNoteService archiveNoteService)
        {
            _archiveNoteService = archiveNoteService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_archiveNoteService.GetAll(_userId));
        }

        [HttpPost]
        public IActionResult Archive(int noteId)
        {
            var success = _archiveNoteService.Archive(_userId, noteId);

            return success ? NoContent() : BadRequest("Invalid noteId");
        }

        [HttpDelete("noteId")]
        public IActionResult Unarchive(int noteId)
        {
            var success = _archiveNoteService.Unarchive(_userId, noteId);

            return success ? NoContent() : BadRequest("Invalid noteId");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Services;

namespace Noteapp.Api.Controllers
{
    [Route("api/lock")]
    [ApiController]
    public class LockNoteController : ControllerBase
    {
        // hardcode user id for now
        private const int _userId = 1;
        private readonly LockNoteService _lockNoteService;
        public LockNoteController(LockNoteService lockNoteService)
        {
            _lockNoteService = lockNoteService;
        }

        [HttpPut("{id}")]
        public IActionResult Lock(int id, [FromBody]bool @lock)
        {
            var success = _lockNoteService.Lock(_userId, id, @lock);

            return success ? NoContent() : BadRequest("Invalid noteId");
        }

    }
}

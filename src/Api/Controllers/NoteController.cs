using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Filters;
using Noteapp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Noteapp.Api.Controllers
{
    [NoteExceptionFilter]
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _noteService;

        public NoteController(NoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public IActionResult GetAll(bool? archived = null)
        {
            var notes = _noteService.GetAll(GetUserId(), archived);
            return Ok(notes);
        }

        // just for testing
        [HttpGet("forall")]
        public IActionResult GetAllForAll()
        {
            return Ok(_noteService.GetAllForAll());
        }

        [HttpPost]
        public IActionResult Create(CreateNoteDto dto)
        {
            var note = _noteService.Create(GetUserId(), dto.Text);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        // might want to return created notes instead of NoContent
        [HttpPost("bulk")]
        public IActionResult BulkCreate(IEnumerable<CreateNoteDto> dtos)
        {
            _noteService.BulkCreate(GetUserId(), dtos.Select(dto => dto.Text));
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var note = _noteService.Get(GetUserId(), id);
            return Ok(note);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            _noteService.Update(GetUserId(), id, dto.Text);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _noteService.Delete(GetUserId(), id);
            return NoContent();
        }

        [HttpPut("{id:int}/lock")]
        public IActionResult Lock(int id)
        {
            _noteService.Lock(GetUserId(), id);
            return NoContent();
        }

        [HttpDelete("{id:int}/lock")]
        public IActionResult Unlock(int id)
        {
            _noteService.Unlock(GetUserId(), id);
            return NoContent();
        }

        [HttpPut("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            _noteService.Archive(GetUserId(), id);
            return NoContent();
        }

        [HttpDelete("{id:int}/archive")]
        public IActionResult Unarchive(int id)
        {
            _noteService.Unarchive(GetUserId(), id);
            return NoContent();
        }

        [HttpPut("{id:int}/pin")]
        public IActionResult Pin(int id)
        {
            _noteService.Pin(GetUserId(), id);
            return NoContent();
        }

        [HttpDelete("{id:int}/pin")]
        public IActionResult Unpin(int id)
        {
            _noteService.Unpin(GetUserId(), id);
            return NoContent();
        }

        [HttpPut("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            var url = _noteService.Publish(GetUserId(), id);
            return CreatedAtAction(nameof(GetPublishedNoteText), new { url = url }, null);
        }

        [HttpDelete("{id:int}/publish")]
        public IActionResult Unpublish(int id)
        {
            _noteService.Unpublish(GetUserId(), id);
            return NoContent();
        }

        [HttpGet("/p/{url}")]
        public IActionResult GetPublishedNoteText(string url)
        {
            var text = _noteService.GetPublishedNoteText(url);
            return Ok(text);
        }

        [HttpGet("{id:int}/snapshots")]
        public IActionResult GetAllSnapshots(int id)
        {
            var snapshots = _noteService.GetAllSnapshots(GetUserId(), id);
            return Ok(snapshots);
        }

        [HttpGet("{noteId:int}/snapshots/{snapshotId:int}")]
        public IActionResult GetSnapshot(int snapshotId, int noteId)
        {
            var snapshot = _noteService.GetSnapshot(GetUserId(), noteId, snapshotId);
            return Ok(snapshot);
        }

        private int GetUserId()
        {
            return User.Identity.IsAuthenticated ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 1;
        }
    }
}

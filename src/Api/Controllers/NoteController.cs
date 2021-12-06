using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Noteapp.Api.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _noteService;
        private const int ANONYMOUS_USER_ID = 1;

        public NoteController(NoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public IActionResult GetAll(bool? archived = null)
        {
            var notes = _noteService.GetAll(GetUserId(), archived);
            var noteDtos = notes.Select(note => new NoteDto(note));
            return Ok(noteDtos);
        }

        [HttpPost]
        public IActionResult Create(CreateNoteDto dto)
        {
            var note = _noteService.Create(GetUserId(), dto.Text);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, new NoteDto(note));
        }

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
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            var note = _noteService.Update(GetUserId(), id, dto.Text);
            return Ok(new NoteDto(note));
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
            var note = _noteService.Lock(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/lock")]
        public IActionResult Unlock(int id)
        {
            var note = _noteService.Unlock(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            var note = _noteService.Archive(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/archive")]
        public IActionResult Unarchive(int id)
        {
            var note = _noteService.Unarchive(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/pin")]
        public IActionResult Pin(int id)
        {
            var note = _noteService.Pin(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/pin")]
        public IActionResult Unpin(int id)
        {
            var note = _noteService.Unpin(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            var note = _noteService.Publish(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/publish")]
        public IActionResult Unpublish(int id)
        {
            var note = _noteService.Unpublish(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpGet("/p/{url}")]
        public IActionResult GetPublishedNoteText(string url)
        {
            string text = _noteService.GetPublishedNoteText(url);
            return Ok(text);
        }

        [HttpGet("{id:int}/snapshots")]
        public IActionResult GetAllSnapshots(int id)
        {
            var snapshots = _noteService.GetAllSnapshots(GetUserId(), id);
            var snapshotDtos = snapshots.Select(snapshot => new NoteSnapshotDto(snapshot));
            return Ok(snapshotDtos);
        }

        private int GetUserId()
        {
            return User.Identity.IsAuthenticated ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : ANONYMOUS_USER_ID;
        }
    }
}

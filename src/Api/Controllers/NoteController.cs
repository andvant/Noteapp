using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAll(bool? archived = null)
        {
            var notes = await _noteService.GetAll(GetUserId(), archived);
            var noteDtos = notes.Select(note => new NoteDto(note));
            return Ok(noteDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteDto dto)
        {
            var note = await _noteService.Create(GetUserId(), dto.Text);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, new NoteDto(note));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(IEnumerable<CreateNoteDto> dtos)
        {
            await _noteService.BulkCreate(GetUserId(), dtos.Select(dto => dto.Text));
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var note = await _noteService.Get(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateNoteDto dto)
        {
            var note = await _noteService.Update(GetUserId(), id, dto.Text);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _noteService.Delete(GetUserId(), id);
            return NoContent();
        }

        [HttpPut("{id:int}/lock")]
        public async Task<IActionResult> Lock(int id)
        {
            var note = await _noteService.Lock(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/lock")]
        public async Task<IActionResult> Unlock(int id)
        {
            var note = await _noteService.Unlock(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var note = await _noteService.Archive(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/archive")]
        public async Task<IActionResult> Unarchive(int id)
        {
            var note = await _noteService.Unarchive(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/pin")]
        public async Task<IActionResult> Pin(int id)
        {
            var note = await _noteService.Pin(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/pin")]
        public async Task<IActionResult> Unpin(int id)
        {
            var note = await _noteService.Unpin(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpPut("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var note = await _noteService.Publish(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpDelete("{id:int}/publish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            var note = await _noteService.Unpublish(GetUserId(), id);
            return Ok(new NoteDto(note));
        }

        [HttpGet("/api/p/{url}")]
        public async Task<IActionResult> GetPublishedNoteText(string url)
        {
            string text = await _noteService.GetPublishedNoteText(url);
            return Ok(text);
        }

        [HttpGet("{id:int}/snapshots")]
        public async Task<IActionResult> GetAllSnapshots(int id)
        {
            var snapshots = await _noteService.GetAllSnapshots(GetUserId(), id);
            var snapshotDtos = snapshots.Select(snapshot => new NoteSnapshotDto(snapshot));
            return Ok(snapshotDtos);
        }

        private int GetUserId()
        {
            return User.Identity.IsAuthenticated ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : ANONYMOUS_USER_ID;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Core.Dtos;
using Noteapp.Core.Services;
using System;
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
            _noteService = noteService ?? throw new ArgumentNullException(nameof(noteService));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var note = await _noteService.Get(GetUserId(), id);
            return Ok(new NoteResponse(note));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(bool? archived = null)
        {
            var notes = await _noteService.GetAll(GetUserId(), archived);
            var noteDtos = notes.Select(note => new NoteResponse(note));
            return Ok(noteDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NoteRequest request)
        {
            var note = await _noteService.Create(GetUserId(), request);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, new NoteResponse(note));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(IEnumerable<NoteRequest> requests)
        {
            await _noteService.BulkCreate(GetUserId(), requests);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NoteRequest request)
        {
            var note = await _noteService.Update(GetUserId(), id, request);
            return Ok(new NoteResponse(note));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _noteService.Delete(GetUserId(), id);
            return NoContent();
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
            var snapshotDtos = snapshots.Select(snapshot => new NoteSnapshotResponse(snapshot));
            return Ok(snapshotDtos);
        }

        private int GetUserId()
        {
            return User.Identity.IsAuthenticated ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : ANONYMOUS_USER_ID;
        }
    }
}

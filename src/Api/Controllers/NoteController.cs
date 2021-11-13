using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Entities;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        // hardcode user id for now
        private const int _userId = 1;
        private readonly NoteService _noteService;

        public NoteController(NoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_noteService.GetAll(_userId));
        }

        [HttpPost]
        public IActionResult Create(CreateNoteDto dto)
        {
            var note = _noteService.Create(_userId, dto.Text);

            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var note = _noteService.TryGet(_userId, id);

            return note != null ? Ok(note) : NotFound();
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            bool success = _noteService.TryUpdate(_userId, id, dto.Text);

            return success ? NoContent() : NotFound(); // TODO: check for note locked error
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            bool success = _noteService.TryDelete(_userId, id);

            return success ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/lock")]
        public IActionResult Lock(int id)
        {
            var success = _noteService.Lock(_userId, id);

            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}/lock")]
        public IActionResult Unlock(int id)
        {
            var success = _noteService.Unlock(_userId, id);

            return success ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            var success = _noteService.Archive(_userId, id);

            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}/archive")]
        public IActionResult Unarchive(int id)
        {
            var success = _noteService.Unarchive(_userId, id);

            return success ? NoContent() : NotFound();
        }

        [HttpPut("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            var url = _noteService.Publish(_userId, id);

            return url != null ? CreatedAtAction(nameof(GetPublishedNoteText), new { url = url }, null) : NotFound();
        }

        [HttpDelete("{id:int}/publish")]
        public IActionResult Unpublish(int id)
        {
            var success = _noteService.Unpublish(_userId, id);

            return success ? NoContent() : NotFound();

        }

        [HttpGet("/p/{url}")]
        public IActionResult GetPublishedNoteText(string url)
        {
            var text = _noteService.GetPublishedNoteText(url);

            return text != null ? Ok(text) : NotFound();
        }
    }

}

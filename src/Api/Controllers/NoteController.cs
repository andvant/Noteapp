using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
using Noteapp.Api.Filters;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Controllers
{
    [NoteExceptionFilter]
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
        public IActionResult GetAll(bool? archived = null)
        {
            var notes = _noteService.GetAll(_userId, archived);
            return Ok(notes);
        }

        [HttpPost]
        public IActionResult Create(CreateNoteDto dto)
        {
            var note = _noteService.Create(_userId, dto.Text);
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        // might want to return created notes instead of NoContent
        [HttpPost("bulk")]
        public IActionResult BulkCreate(BulkCreateNoteDto dto)
        {
            _noteService.BulkCreate(_userId, dto.Texts);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var note = _noteService.Get(_userId, id);
            return Ok(note);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            _noteService.Update(_userId, id, dto.Text);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _noteService.Delete(_userId, id);
            return NoContent();
        }

        [HttpPut("{id:int}/lock")]
        public IActionResult Lock(int id)
        {
            _noteService.Lock(_userId, id);
            return NoContent();
        }

        [HttpDelete("{id:int}/lock")]
        public IActionResult Unlock(int id)
        {
            _noteService.Unlock(_userId, id);
            return NoContent();
        }

        [HttpPut("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            _noteService.Archive(_userId, id);
            return NoContent();
        }

        [HttpDelete("{id:int}/archive")]
        public IActionResult Unarchive(int id)
        {
            _noteService.Unarchive(_userId, id);
            return NoContent();
        }

        [HttpPut("{id:int}/pin")]
        public IActionResult Pin(int id)
        {
            _noteService.Pin(_userId, id);
            return NoContent();
        }

        [HttpDelete("{id:int}/pin")]
        public IActionResult Unpin(int id)
        {
            _noteService.Unpin(_userId, id);
            return NoContent();
        }

        [HttpPut("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            var url = _noteService.Publish(_userId, id);
            return CreatedAtAction(nameof(GetPublishedNoteText), new { url = url }, null);
        }

        [HttpDelete("{id:int}/publish")]
        public IActionResult Unpublish(int id)
        {
            _noteService.Unpublish(_userId, id);
            return NoContent();
        }

        [HttpGet("/p/{url}")]
        public IActionResult GetPublishedNoteText(string url)
        {
            var text = _noteService.GetPublishedNoteText(url);
            return Ok(text);
        }
    }
}

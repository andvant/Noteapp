using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
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

        // TODO: return 413 if Payload too large
        // also might want to return created notes instead of NoContent
        [HttpPost("bulk")]
        public IActionResult BulkCreate(BulkCreateNoteDto dto)
        {
            _noteService.BulkCreate(_userId, dto.Texts);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var note = _noteService.Get(_userId, id);
                return Ok(note);
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            try
            {
                _noteService.Update(_userId, id, dto.Text);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (NoteLockedException)
            {
                return Forbid(); // might want to change the status code
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _noteService.Delete(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:int}/lock")]
        public IActionResult Lock(int id)
        {
            try
            {
                _noteService.Lock(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/lock")]
        public IActionResult Unlock(int id)
        {
            try
            {
                _noteService.Unlock(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:int}/archive")]
        public IActionResult Archive(int id)
        {
            try
            {
                _noteService.Archive(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/archive")]
        public IActionResult Unarchive(int id)
        {
            try
            {
                _noteService.Unarchive(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:int}/pin")]
        public IActionResult Pin(int id)
        {
            try
            {
                _noteService.Pin(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/pin")]
        public IActionResult Unpin(int id)
        {
            try
            {
                _noteService.Unpin(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:int}/publish")]
        public IActionResult Publish(int id)
        {
            try
            {
                var url = _noteService.Publish(_userId, id);
                return CreatedAtAction(nameof(GetPublishedNoteText), new { url = url }, null);
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/publish")]
        public IActionResult Unpublish(int id)
        {
            try
            {
                _noteService.Unpublish(_userId, id);
                return NoContent();
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/p/{url}")]
        public IActionResult GetPublishedNoteText(string url)
        {
            try
            {
                var text = _noteService.GetPublishedNoteText(url);
                return Ok(text);
            }
            catch (NoteNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

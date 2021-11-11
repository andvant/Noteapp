using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Dtos;
using Noteapp.Api.Entities;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Controllers
{
    [Route("api/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _noteRepository;
        private const int _userId = 1;

        public NoteController(NoteService noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_noteRepository.GetAll(_userId));
        }

        [HttpPost]
        public IActionResult Create(CreateNoteDto dto)
        {
            var note = _noteRepository.Create(_userId, dto.Text);

            return CreatedAtRoute("Get", new { id = note.Id }, note);
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var note = _noteRepository.TryGet(_userId, id);

            return note != null ? Ok(note) : BadRequest("Invalid note id");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            bool success = _noteRepository.TryUpdate(_userId, id, dto.Text);

            return success ? NoContent() : BadRequest("Invalid note id");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = _noteRepository.TryDelete(_userId, id);

            return success ? NoContent() : BadRequest("Invalid note id");
        }
    }
}

﻿using Microsoft.AspNetCore.Http;
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
        private readonly NoteService _noteService;
        // hardcode user id for now
        private const int _userId = 1;

        public NoteController(NoteService noteRepository)
        {
            _noteService = noteRepository;
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

            return CreatedAtRoute("Get", new { id = note.Id }, note);
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var note = _noteService.TryGet(_userId, id);

            return note != null ? Ok(note) : BadRequest("Invalid noteId");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateNoteDto dto)
        {
            bool success = _noteService.TryUpdate(_userId, id, dto.Text);

            return success ? NoContent() : BadRequest("Invalid noteId or the note is locked");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool success = _noteService.TryDelete(_userId, id);

            return success ? NoContent() : BadRequest("Invalid noteId");
        }
    }
}

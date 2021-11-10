using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteRepository _noteRepository;
        private const int _userId = 1;

        public NoteController(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_noteRepository.GetAll(_userId));
        }
    }
}

﻿using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public class NoteRepository
    {
        public List<Note> Notes { get; set; }

        public NoteRepository(bool seed = true)
        {
            Notes = seed ? GetInMemoryNotes() : new List<Note>();
        }

        private List<Note> GetInMemoryNotes()
        {
            const int userId = 1;

            var notes = new List<Note>()
            {
                new()
                {
                    Id = 1,
                    Text = "note 1",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 2,
                    Text = "note 2",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                },
                new()
                {
                    Id = 3,
                    Text = "note 3",
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    AuthorId = userId
                }
            };

            return notes;
        }
    }
}

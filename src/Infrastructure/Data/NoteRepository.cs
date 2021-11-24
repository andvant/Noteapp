using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Noteapp.Infrastructure.Data
{
    public class NoteRepository : INoteRepository
    {
        public List<Note> Notes { get; set; } = new();
    }
}

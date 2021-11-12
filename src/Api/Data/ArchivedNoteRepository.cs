using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public class ArchivedNoteRepository
    {
        public List<ArchivedNote> ArchivedNotes { get; set; } = new();
    }
}

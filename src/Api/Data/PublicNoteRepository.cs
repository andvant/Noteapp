using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public class PublicNoteRepository
    {
        public List<PublicNote> PublicNotes { get; set; } = new();
    }
}

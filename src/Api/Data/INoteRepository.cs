using Noteapp.Api.Entities;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public interface INoteRepository
    {
        List<Note> Notes { get; set; }
    }
}
using Noteapp.Core.Entities;
using System.Collections.Generic;

namespace Noteapp.Core.Interfaces
{
    public interface INoteRepository
    {
        List<Note> Notes { get; set; }
    }
}
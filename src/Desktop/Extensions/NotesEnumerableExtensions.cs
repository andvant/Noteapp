using Noteapp.Desktop.Models;
using System;
using System.Collections.Generic;

namespace Noteapp.Desktop.Extensions
{
    public static class NotesEnumerableExtensions
    {
        public static IEnumerable<Note> Sort(this IEnumerable<Note> notes, NotesSorting sorting)
        {
            Comparison<Note> comparison = sorting switch
            {
                NotesSorting.ByCreatedAscending => (note1, note2) => DateTime.Compare(note1.Created, note2.Created),
                NotesSorting.ByCreatedDescending => (note1, note2) => DateTime.Compare(note2.Created, note1.Created),
                NotesSorting.ByUpdatedAscending => (note1, note2) => DateTime.Compare(note1.UpdatedLocal, note2.UpdatedLocal),
                NotesSorting.ByUpdatedDescending => (note1, note2) => DateTime.Compare(note2.UpdatedLocal, note1.UpdatedLocal),
                NotesSorting.ByTextAscending => (note1, note2) => string.Compare(note1.Text, note2.Text,
                    StringComparison.CurrentCultureIgnoreCase),
                NotesSorting.ByTextDescending => (note1, note2) => string.Compare(note2.Text, note1.Text,
                    StringComparison.CurrentCultureIgnoreCase),
                _ => (_, _) => 0
            };

            var sortedNotes = new List<Note>(notes);
            sortedNotes.Sort(comparison);
            return sortedNotes;
        }
    }
}

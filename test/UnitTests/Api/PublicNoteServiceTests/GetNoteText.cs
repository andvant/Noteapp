using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.PublicNoteServiceTests
{
    public class GetNoteText
    {
        [Fact]
        public void ReturnsNoteTextGivenRightUrl()
        {
            // Arrange
            var publicNoteRepository = new PublicNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new PublicNoteService(publicNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                Text = "note 1"
            };
            var publicNote = new PublicNote
            {
                Id = 1,
                Url = "testtest",
                NoteId = note.Id,
                Note = note
            };
            noteRepository.Notes.Add(note);
            publicNoteRepository.PublicNotes.Add(publicNote);

            // Act
            var text = service.GetNoteText("testtest");

            // Assert
            Assert.Equal("note 1", text);
        }

        [Fact]
        public void ReturnsNullGivenWrongUrl()
        {
            // Arrange
            var publicNoteRepository = new PublicNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new PublicNoteService(publicNoteRepository, noteRepository);

            // Act
            var text = service.GetNoteText("shouldfail");

            // Assert
            Assert.Null(text);
        }

    }
}

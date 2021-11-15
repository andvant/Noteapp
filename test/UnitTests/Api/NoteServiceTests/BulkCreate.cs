using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class BulkCreate
    {
        [Fact]
        public void CreatesNewNotes()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var dateTime = new DateTime(2021, 1, 1);
            var noteService = new NoteService(noteRepository, new DateTimeProvider(dateTime));
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            var newNotesTexts = new List<string>
            {
                "note2",
                "note3",
                "note4"
            };

            // Act
            noteService.BulkCreate(1, newNotesTexts);

            // Assert
            Assert.Equal(newNotesTexts.Count + 1, noteRepository.Notes.Count);
            Assert.Equal(dateTime, noteRepository.Notes[1].Created);
            Assert.Equal(dateTime, noteRepository.Notes[1].Updated);

            Assert.Equal(2, noteRepository.Notes[1].Id);
            Assert.Equal(1, noteRepository.Notes[1].AuthorId);
            Assert.Equal("note2", noteRepository.Notes[1].Text);

            Assert.Equal(3, noteRepository.Notes[2].Id);
            Assert.Equal(1, noteRepository.Notes[2].AuthorId);
            Assert.Equal("note3", noteRepository.Notes[2].Text);

            Assert.Equal(4, noteRepository.Notes[3].Id);
            Assert.Equal(1, noteRepository.Notes[3].AuthorId);
            Assert.Equal("note4", noteRepository.Notes[3].Text);
        }
    }
}

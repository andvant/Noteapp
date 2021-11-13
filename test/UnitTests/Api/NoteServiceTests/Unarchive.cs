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
    public class Unarchive
    {
        [Fact]
        public void UnarchivesNoteGivenValidNoteIdAndUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Archived = true
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);

            // Act
            var result = service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.True(result);
            Assert.False(note2.Archived);
            Assert.True(note1.Archived);
        }

        [Fact]
        public void ReturnsFalseGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            noteRepository.Notes.Add(note);

            // Act
            var result = service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.False(result);
            Assert.True(note.Archived);
        }

        [Fact]
        public void ReturnsFalseGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            noteRepository.Notes.Add(note);

            // Act
            var result = service.Unarchive(userId: 2, noteId: 1);

            // Assert
            Assert.False(result);
            Assert.True(note.Archived);
        }
    }
}

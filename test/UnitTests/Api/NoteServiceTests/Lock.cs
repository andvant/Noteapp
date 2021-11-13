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
    public class Lock
    {
        [Fact]
        public void LocksNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.Lock(userId: 1, noteId: 1);

            // Assert
            Assert.True(success);
            Assert.True(note.Locked);
        }

        [Fact]
        public void ReturnsFalseNoteGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.Lock(userId: 1, noteId: 2);

            // Assert
            Assert.False(success);
            Assert.False(note.Locked);
        }

        [Fact]
        public void DoesNotLockNoteGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.Lock(userId: 2, noteId: 1);

            // Assert
            Assert.False(success);
            Assert.False(note.Locked);
        }
    }
}

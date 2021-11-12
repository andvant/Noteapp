using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.LockNoteServiceTests
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
            var lockNoteService = new LockNoteService(noteRepository);

            // Act
            var success = lockNoteService.Lock(userId: 1, noteId: 1, @lock: true);

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
            var lockNoteService = new LockNoteService(noteRepository);

            // Act
            var success = lockNoteService.Lock(userId: 1, noteId: 2, @lock: true);

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
            var lockNoteService = new LockNoteService(noteRepository);

            // Act
            var success = lockNoteService.Lock(userId: 2, noteId: 1, @lock: true);

            // Assert
            Assert.False(success);
            Assert.False(note.Locked);
        }

        [Fact]
        public void UnlocksNote()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            noteRepository.Notes.Add(note);
            var lockNoteService = new LockNoteService(noteRepository);

            // Act
            var success = lockNoteService.Lock(userId: 1, noteId: 1, @lock: false);

            // Assert
            Assert.True(success);
            Assert.False(note.Locked);
        }
    }
}

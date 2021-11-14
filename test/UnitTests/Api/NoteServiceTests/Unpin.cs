using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
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
    public class Unpin
    {
        [Fact]
        public void UnpinsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Pinned = true
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);

            // Act
            service.Unpin(userId: 1, noteId: 2);

            // Assert
            Assert.False(note2.Pinned);
            Assert.True(note1.Pinned);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Unpin(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Unpin(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }
    }
}

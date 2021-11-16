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
    public class Unarchive
    {
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void UnarchivesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
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
            _noteRepository.Notes.Add(note1);
            _noteRepository.Notes.Add(note2);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.False(note2.Archived);
            Assert.True(note1.Archived);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Archived);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.Unarchive(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Archived);
        }
    }
}

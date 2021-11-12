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
    public class PublishNote
    {
        [Fact]
        public void ReturnsNullGivenNonExistentNoteId()
        {
            // Arrange
            var publicNoteRepository = new PublicNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new PublicNoteService(publicNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var url = service.PublishNote(userId: 1, noteId: 2);

            // Assert
            Assert.Null(url);
        }

        [Fact]
        public void ReturnsNullGivenWrongUserId()
        {
            // Arrange
            var publicNoteRepository = new PublicNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new PublicNoteService(publicNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var url = service.PublishNote(userId: 2, noteId: 1);

            // Assert
            Assert.Null(url);
        }

        [Fact]
        public void ReturnsNewUrlGivenValidUserIdAndNoteId()
        {
            // Arrange
            var publicNoteRepository = new PublicNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new PublicNoteService(publicNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var url = service.PublishNote(userId: 1, noteId: 1);

            // Assert
            Assert.NotNull(url);
            Assert.Equal(url, publicNoteRepository.PublicNotes.Single().Url);
        }
    }
}

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
    public class Archive
    {
        [Fact]
        public void ArchivesNoteGivenValidNoteIdAndUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            noteRepository.Notes.Add(note);

            // Act
            service.Archive(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Archived);
        }

        [Fact]
        public void DoesNotArchiveGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Archive(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }

        [Fact]
        public void DoesNotArchiveGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Archive(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }
    }
}

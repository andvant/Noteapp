using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.ArchiveNoteServiceTests
{
    public class Archive
    {
        [Fact]
        public void ArchivesNoteGivenValidNoteIdAndUserId()
        {
            // Arrange
            var archivedNoteRepository = new ArchivedNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new ArchiveNoteService(archivedNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var result = service.Archive(userId: 1, noteId: 1);

            // Assert
            Assert.True(result);
            Assert.Equal(note.Id, archivedNoteRepository.ArchivedNotes.Single().NoteId);
        }

        [Fact]
        public void DoesNotArchiveGivenNonExistentNoteId()
        {
            // Arrange
            var archivedNoteRepository = new ArchivedNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new ArchiveNoteService(archivedNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var result = service.Archive(userId: 1, noteId: 2);

            // Assert
            Assert.False(result);
            Assert.Empty(archivedNoteRepository.ArchivedNotes);
        }

        [Fact]
        public void DoesNotArchiveGivenWrongUserId()
        {
            // Arrange
            var archivedNoteRepository = new ArchivedNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new ArchiveNoteService(archivedNoteRepository, noteRepository);

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var result = service.Archive(userId: 2, noteId: 1);

            // Assert
            Assert.False(result);
            Assert.Empty(archivedNoteRepository.ArchivedNotes);
        }
    }
}

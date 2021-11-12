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
    public class Unarchive
    {
        [Fact]
        public void UnarchivesNoteGivenValidNoteIdAndUserId()
        {
            // Arrange
            var archivedNoteRepository = new ArchivedNoteRepository();
            var noteRepository = new NoteRepository(false);
            var service = new ArchiveNoteService(archivedNoteRepository, noteRepository);

            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);

            var archivedNote1 = new ArchivedNote()
            {
                Id = 10,
                Note = note1,
                NoteId = note1.Id
            };
            var archivedNote2 = new ArchivedNote()
            {
                Id = 20,
                Note = note2,
                NoteId = note2.Id
            };
            archivedNoteRepository.ArchivedNotes.Add(archivedNote1);
            archivedNoteRepository.ArchivedNotes.Add(archivedNote2);

            // Act
            var result = service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.True(result);
            Assert.Equal(note1.Id, archivedNoteRepository.ArchivedNotes.Single().NoteId);
        }

        [Fact]
        public void ReturnsFalseGivenNonExistentNoteId()
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

            var archivedNote = new ArchivedNote()
            {
                Id = 10,
                Note = note,
                NoteId = note.Id
            };
            archivedNoteRepository.ArchivedNotes.Add(archivedNote);

            // Act
            var result = service.Unarchive(userId: 1, noteId: 2);

            // Assert
            Assert.False(result);
            Assert.Equal(note.Id, archivedNoteRepository.ArchivedNotes.Single().NoteId);
        }

        [Fact]
        public void ReturnsFalseGivenWrongUserId()
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

            var archivedNote = new ArchivedNote()
            {
                Id = 10,
                Note = note,
                NoteId = note.Id
            };
            archivedNoteRepository.ArchivedNotes.Add(archivedNote);

            // Act
            var result = service.Unarchive(userId: 2, noteId: 1);

            // Assert
            Assert.False(result);
            Assert.Equal(note.Id, archivedNoteRepository.ArchivedNotes.Single().NoteId);
        }
    }
}

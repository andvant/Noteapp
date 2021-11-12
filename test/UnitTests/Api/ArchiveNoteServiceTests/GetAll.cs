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
    public class GetAll
    {
        [Fact]
        public void ReturnsAllArchivedNotesForGivenUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var archivedNoteRepository = new ArchivedNoteRepository();
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);

            var archivedNote1 = new ArchivedNote()
            {
                Id = 1,
                Note = note1,
                NoteId = note1.Id
            };
            var archivedNote2 = new ArchivedNote()
            {
                Id = 2,
                Note = note2,
                NoteId = note2.Id
            };
            var archivedNote3 = new ArchivedNote()
            {
                Id = 3,
                Note = note3,
                NoteId = note3.Id
            };
            archivedNoteRepository.ArchivedNotes.Add(archivedNote1);
            archivedNoteRepository.ArchivedNotes.Add(archivedNote2);
            archivedNoteRepository.ArchivedNotes.Add(archivedNote3);
            var archiveNoteService = new ArchiveNoteService(archivedNoteRepository, noteRepository);

            // Act
            var returnedNotes = archiveNoteService.GetAll(userId: 1);

            // Assert
            Assert.Equal(2, returnedNotes.Count());
            Assert.Contains(archivedNote1, returnedNotes);
            Assert.Contains(archivedNote2, returnedNotes);
            Assert.DoesNotContain(archivedNote3, returnedNotes);
        }
    }
}

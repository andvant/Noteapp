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
    public class GetAll
    {
        [Fact]
        public void ReturnsAllNotesForGivenUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
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
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);
            noteRepository.Notes.Add(note3);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, null);

            // Assert
            Assert.Equal(2, returnedNotes.Count());
            Assert.Contains(note1, returnedNotes);
            Assert.Contains(note2, returnedNotes);
        }

        [Fact]
        public void ReturnsOnlyArchivedNotesForGivenUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
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
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);
            noteRepository.Notes.Add(note3);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, true);

            // Assert
            Assert.Single(returnedNotes);
            Assert.Contains(note1, returnedNotes);
        }

        [Fact]
        public void ReturnsOnlyNonArchivedNotesForGivenUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
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
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);
            noteRepository.Notes.Add(note3);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, false);

            // Assert
            Assert.Single(returnedNotes);
            Assert.Contains(note2, returnedNotes);
        }
    }
}

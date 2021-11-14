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
    public class Get
    {
        [Fact]
        public void ReturnsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var returnedNote = noteService.Get(1, 1);

            // Assert
            Assert.Same(note, returnedNote);
        }
            
        [Fact]
        public void ReturnsNullGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            Action act = () => noteService.Get(1, 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

        [Fact]
        public void ReturnsNullGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            Action act = () => noteService.Get(2, 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }
    }
}

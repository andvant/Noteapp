using Noteapp.Api.Data;
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
    public class Create
    {
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void CreatesNewNote()
        {
            // Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider(dateTime));

            // Act
            var createdNote = noteService.Create(userId: 1, text: "new note");

            // Assert
            Assert.Same(createdNote, _noteRepository.Notes.Single());
            Assert.Equal(dateTime, createdNote.Created);
            Assert.Equal(dateTime, createdNote.Updated);
            Assert.Equal(1, createdNote.Id);
            Assert.Equal(1, createdNote.AuthorId);
            Assert.Equal("new note", createdNote.Text);
        }
    }
}

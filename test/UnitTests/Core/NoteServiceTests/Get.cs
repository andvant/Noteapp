using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Get
    {
        private readonly Mock<IRepository<Note>> _mock = new Mock<IRepository<Note>>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ReturnsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var returnedNote = noteService.Get(userId: 1, noteId: 1);

            // Assert
            Assert.Same(note, returnedNote);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Get(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Get(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }
    }
}

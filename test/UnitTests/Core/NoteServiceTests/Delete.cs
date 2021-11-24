using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Delete
    {
        private readonly Mock<IRepository<Note>> _mock = new Mock<IRepository<Note>>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void DeletesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 2,
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note1);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Delete(userId: 1, noteId: 1);

            // Assert
            _mock.Verify(repo => repo.Delete(note1), Times.Once);
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
            Action act = () => noteService.Delete(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            _mock.Verify(repo => repo.Delete(It.IsAny<Note>()), Times.Never);
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
            Action act = () => noteService.Delete(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            _mock.Verify(repo => repo.Delete(It.IsAny<Note>()), Times.Never);
        }
    }
}

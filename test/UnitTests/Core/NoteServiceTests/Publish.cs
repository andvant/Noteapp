using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Publish
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ReturnsUrlGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Publish(userId: 1, noteId: 1);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(note.PublicUrl));
            _mock.Verify(repo => repo.Update(note), Times.Once);
        }


        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Publish(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(note.PublicUrl);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Publish(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(note.PublicUrl);
        }


    }
}

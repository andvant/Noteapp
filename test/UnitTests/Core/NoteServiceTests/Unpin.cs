using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Unpin
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void UnpinsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Pinned = true
            };
            _mock.Setup(repo => repo.Find(1, false)).Returns(note1);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Unpin(userId: 1, noteId: 1);

            // Assert
            Assert.False(note1.Pinned);
            Assert.True(note2.Pinned);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            _mock.Setup(repo => repo.Find(1, false)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unpin(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            _mock.Setup(repo => repo.Find(1, false)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unpin(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }
    }
}

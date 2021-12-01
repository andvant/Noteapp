using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Unlock
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void UnlocksNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Unlock(userId: 1, noteId: 1);

            // Assert
            Assert.False(note.Locked);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unlock(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Locked);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unlock(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Locked);
        }
    }
}

using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Archive
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ArchivesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Archive(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Archived);
            _mock.Verify(repo => repo.Update(note), Times.Once);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Archive(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Archive(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }
    }
}

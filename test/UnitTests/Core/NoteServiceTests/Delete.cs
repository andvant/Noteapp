using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Delete
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public async Task DeletesNoteGivenValidUserIdAndNoteId()
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
            _mock.Setup(repo => repo.FindWithoutSnapshots(1)).ReturnsAsync(note1);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Delete(userId: 1, noteId: 1);

            // Assert
            _mock.Verify(repo => repo.Delete(note1), Times.Once);
        }

        [Fact]
        public async Task ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            _mock.Setup(repo => repo.FindWithoutSnapshots(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Delete(userId: 1, noteId: 2);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            _mock.Verify(repo => repo.Delete(It.IsAny<Note>()), Times.Never);
        }

        [Fact]
        public async Task ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            _mock.Setup(repo => repo.FindWithoutSnapshots(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Delete(userId: 2, noteId: 1);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            _mock.Verify(repo => repo.Delete(It.IsAny<Note>()), Times.Never);
        }
    }
}

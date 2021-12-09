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
    public class Unpin
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public async Task UnpinsNoteGivenValidUserIdAndNoteId()
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
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note1);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Unpin(userId: 1, noteId: 1);

            // Assert
            Assert.False(note1.Pinned);
            Assert.True(note2.Pinned);
        }

        [Fact]
        public async Task ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Unpin(userId: 1, noteId: 2);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }

        [Fact]
        public async Task ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Unpin(userId: 2, noteId: 1);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.True(note.Pinned);
        }
    }
}

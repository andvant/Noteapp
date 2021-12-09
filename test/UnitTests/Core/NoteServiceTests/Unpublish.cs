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
    public class Unpublish
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public async Task UnpublishesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Unpublish(userId: 1, noteId: 1);

            // Assert
            Assert.Null(note.PublicUrl);
        }


        [Fact]
        public async Task ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Unpublish(userId: 1, noteId: 2);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.Equal("testtest", note.PublicUrl);
        }

        [Fact]
        public async Task ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Unpublish(userId: 2, noteId: 1);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.Equal("testtest", note.PublicUrl);
        }
    }
}

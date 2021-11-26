using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class GetAll
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ReturnsAllNotesForGivenUserId()
        {
            // Arrange
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            _mock.Setup(repo => repo.FindByAuthorId(1)).Returns(new[] { note1, note2 });
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, archived: null);

            // Assert
            Assert.Equal(2, returnedNotes.Count());
            Assert.Contains(note1, returnedNotes);
            Assert.Contains(note2, returnedNotes);
        }

        [Fact]
        public void ReturnsOnlyArchivedNotesForGivenUserId()
        {
            // Arrange
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            _mock.Setup(repo => repo.FindByAuthorId(1)).Returns(new[] { note1, note2 });
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, archived: true);

            // Assert
            Assert.Single(returnedNotes);
            Assert.Contains(note1, returnedNotes);
        }

        [Fact]
        public void ReturnsOnlyNonArchivedNotesForGivenUserId()
        {
            // Arrange
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = true
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 1,
                Archived = false
            };
            var note3 = new Note()
            {
                Id = 3,
                AuthorId = 2,
                Archived = false
            };
            _mock.Setup(repo => repo.FindByAuthorId(1)).Returns(new[] { note1, note2 });
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, archived: false);

            // Assert
            Assert.Single(returnedNotes);
            Assert.Contains(note2, returnedNotes);
        }
    }
}

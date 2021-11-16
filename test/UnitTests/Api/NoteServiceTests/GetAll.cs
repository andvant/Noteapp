using Moq;
using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class GetAll
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public GetAll()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

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
            _noteRepository.Notes.Add(note1);
            _noteRepository.Notes.Add(note2);
            _noteRepository.Notes.Add(note3);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

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
            _noteRepository.Notes.Add(note1);
            _noteRepository.Notes.Add(note2);
            _noteRepository.Notes.Add(note3);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

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
            _noteRepository.Notes.Add(note1);
            _noteRepository.Notes.Add(note2);
            _noteRepository.Notes.Add(note3);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            var returnedNotes = noteService.GetAll(userId: 1, archived: false);

            // Assert
            Assert.Single(returnedNotes);
            Assert.Contains(note2, returnedNotes);
        }
    }
}

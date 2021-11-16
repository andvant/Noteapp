using Moq;
using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class Pin
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public Pin()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

        [Fact]
        public void PinsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            service.Pin(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => service.Pin(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => service.Pin(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Pinned);
        }
    }
}

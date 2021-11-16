using Moq;
using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class Update
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _created = new DateTime(2021, 1, 1);
        private readonly DateTime _updated = new DateTime(2021, 2, 2);

        public Update()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == _updated);
        }

        [Fact]
        public void UpdatesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Equal("updated text", _noteRepository.Notes.Single().Text);
            Assert.Equal(_updated, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(_created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 2, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(_created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(_created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 2, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(_created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(_created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNoteIsLocked()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text",
                Locked = true
            };
            _noteRepository.Notes.Add(note);

            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteLockedException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(_created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(_created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }
    }
}

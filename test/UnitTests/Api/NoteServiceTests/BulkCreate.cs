using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class BulkCreate
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _dateTime = new DateTime(2021, 1, 1);

        public BulkCreate()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == _dateTime);
        }

        [Fact]
        public void CreatesNewNotes()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            var newNotesTexts = new List<string>
            {
                "note2",
                "note3",
                "note4"
            };

            // Act
            noteService.BulkCreate(1, newNotesTexts);

            // Assert
            Assert.Equal(newNotesTexts.Count + 1, _noteRepository.Notes.Count);
            Assert.Equal(_dateTime, _noteRepository.Notes[1].Created);
            Assert.Equal(_dateTime, _noteRepository.Notes[1].Updated);

            Assert.Equal(2, _noteRepository.Notes[1].Id);
            Assert.Equal(1, _noteRepository.Notes[1].AuthorId);
            Assert.Equal("note2", _noteRepository.Notes[1].Text);

            Assert.Equal(3, _noteRepository.Notes[2].Id);
            Assert.Equal(1, _noteRepository.Notes[2].AuthorId);
            Assert.Equal("note3", _noteRepository.Notes[2].Text);

            Assert.Equal(4, _noteRepository.Notes[3].Id);
            Assert.Equal(1, _noteRepository.Notes[3].AuthorId);
            Assert.Equal("note4", _noteRepository.Notes[3].Text);
        }

        [Fact]
        public void ThrowsGivenTooManyNotes()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            var newNotesTexts = Enumerable.Repeat("note", 21);

            // Act
            Action act = () => noteService.BulkCreate(userId: 1, texts: newNotesTexts);

            // Assert
            Assert.Throws<TooManyNotesException>(act);
            Assert.Single(_noteRepository.Notes);
        }
    }
}

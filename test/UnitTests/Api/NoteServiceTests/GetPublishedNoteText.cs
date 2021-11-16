using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class GetPublishedNoteText
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public GetPublishedNoteText()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

        [Fact]
        public void ReturnsNoteTextGivenRightUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                Text = "note 1",
                PublicUrl = "testtest"
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            var text = noteService.GetPublishedNoteText("testtest");

            // Assert
            Assert.Equal("note 1", text);
        }

        [Fact]
        public void ThrowsGivenWrongUrl()
        {
            // Arrange
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.GetPublishedNoteText("shouldfail");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

    }
}

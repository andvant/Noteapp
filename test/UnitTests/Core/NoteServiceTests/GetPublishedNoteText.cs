using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class GetPublishedNoteText
    {
        private readonly Mock<IRepository<Note>> _mock = new Mock<IRepository<Note>>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ReturnsNoteTextGivenValidUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                Text = "note 1",
                PublicUrl = "testtest"
            };
            _mock.Setup(repo => repo.Find(note => note.PublicUrl == "testtest")).Returns(new[] { note });
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var text = noteService.GetPublishedNoteText("testtest");

            // Assert
            Assert.Equal("note 1", text);
        }

        [Fact]
        public void ThrowsGivenWrongUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                Text = "note 1",
                PublicUrl = "testtest"
            };
            _mock.Setup(repo => repo.Find(note => note.PublicUrl == "testtest")).Returns(new[] { note });
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.GetPublishedNoteText("shouldfail");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

    }
}

using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class GetPublishedNoteText
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void ReturnsNoteTextGivenValidUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                PublicUrl = "testtest"
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "note 1"
            };
            note.CurrentSnapshot = noteSnapshot;

            _mock.Setup(repo => repo.FindByPublicUrl("testtest")).Returns(note);
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
                PublicUrl = "testtest"
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "note 1"
            };
            note.CurrentSnapshot = noteSnapshot;

            _mock.Setup(repo => repo.FindByPublicUrl("testtest")).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.GetPublishedNoteText("shouldfail");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

    }
}

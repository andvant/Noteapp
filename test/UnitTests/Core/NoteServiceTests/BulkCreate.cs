using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class BulkCreate
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public void CreatesNewNotes()
        {
            // Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == dateTime);
            var noteService = new NoteService(_mock.Object, dateTimeProvider);

            var noteTexts = new List<string>
            {
                "note1",
                "note2",
                "note3"
            };

            // Act
            noteService.BulkCreate(1, noteTexts);

            // Assert
            _mock.Verify(repo => repo.AddRange(
                It.Is<List<Note>>(notes =>
                    notes.Count == noteTexts.Count &&
                    notes.Any(note =>
                        note.AuthorId == 1 &&
                        note.Created == dateTime &&
                        note.Updated == dateTime &&
                        noteTexts.Contains(note.Text)))
            ), Times.Once);
        }

        [Fact]
        public void ThrowsGivenTooManyNotes()
        {
            // Arrange
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);
            var newNotesTexts = Enumerable.Repeat("note", 21);

            // Act
            Action act = () => noteService.BulkCreate(userId: 1, texts: newNotesTexts);

            // Assert
            Assert.Throws<TooManyNotesException>(act);
            _mock.VerifyNoOtherCalls();
        }
    }
}

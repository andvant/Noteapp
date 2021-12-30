using Moq;
using Noteapp.Core;
using Noteapp.Core.Dtos;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class BulkCreate
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public async Task CreatesNewNotes()
        {
            // Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == dateTime);
            var noteService = new NoteService(_mock.Object, dateTimeProvider);

            var noteRequests = new List<NoteRequest>
            {
                new() { Text = "note1"},
                new() { Text = "note2"},
                new() { Text = "note3"},
            };

            // Act
            await noteService.BulkCreate(1, noteRequests);

            // Assert
            _mock.Verify(repo => repo.AddRange(
                It.Is<List<Note>>(notes =>
                    notes.Count == noteRequests.Count &&
                    notes.Any(note =>
                        note.AuthorId == 1 &&
                        note.Created == dateTime &&
                        note.Updated == dateTime &&
                        noteRequests.Select(request => request.Text).Contains(note.Text)))
            ), Times.Once);
        }

        [Fact]
        public async Task ThrowsGivenTooManyNotes()
        {
            // Arrange
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);
            var noteRequests = Enumerable.Repeat(new NoteRequest() { Text = "note" }, Constants.MAX_BULK_NOTES + 1);

            // Act
            Func<Task> act = async () => await noteService.BulkCreate(userId: 1, noteRequests);

            // Assert
            await Assert.ThrowsAsync<TooManyNotesException>(act);
            _mock.VerifyNoOtherCalls();
        }
    }
}

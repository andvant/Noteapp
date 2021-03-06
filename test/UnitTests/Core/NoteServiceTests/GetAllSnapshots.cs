using Moq;
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
    public class GetAllSnapshots
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        [Fact]
        public async Task ReturnsAllSnapshotsGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            var snapshot1 = new NoteSnapshot()
            {
                Id = 1,
                NoteId = 1,
                Text = "text 1",
                Created = new DateTime(2021, 1, 1)
            };
            var snapshot2 = new NoteSnapshot()
            {
                Id = 2,
                NoteId = 1,
                Text = "text 2",
                Created = new DateTime(2021, 2, 2)
            };
            note.Snapshots = new List<NoteSnapshot>() { snapshot2, snapshot1 }; // repository returns items in the wrong order
            _mock.Setup(repo => repo.FindWithAllSnapshots(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            var returnedSnapshots = await noteService.GetAllSnapshots(userId: 1, noteId: 1);

            // Assert
            Assert.Contains(snapshot1, returnedSnapshots);
            Assert.Contains(snapshot2, returnedSnapshots);
            Assert.Equal(2, returnedSnapshots.Count());
            Assert.Equal(new[] { snapshot1, snapshot2 }, returnedSnapshots); // items should be ordered by Created property
        }

        [Fact]
        public async Task ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            var snapshot1 = new NoteSnapshot()
            {
                Id = 1,
                NoteId = 1,
                Text = "text 1",
                Created = new DateTime(2021, 1, 1)
            };
            var snapshot2 = new NoteSnapshot()
            {
                Id = 2,
                NoteId = 1,
                Text = "text 2",
                Created = new DateTime(2021, 2, 2)
            };
            note.Snapshots = new List<NoteSnapshot>() { snapshot2, snapshot1 }; // repository returns items in the wrong order
            _mock.Setup(repo => repo.FindWithAllSnapshots(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.GetAllSnapshots(userId: 1, noteId: 2);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
        }

        [Fact]
        public async Task ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            var snapshot1 = new NoteSnapshot()
            {
                Id = 1,
                NoteId = 1,
                Text = "text 1",
                Created = new DateTime(2021, 1, 1)
            };
            var snapshot2 = new NoteSnapshot()
            {
                Id = 2,
                NoteId = 1,
                Text = "text 2",
                Created = new DateTime(2021, 2, 2)
            };
            note.Snapshots = new List<NoteSnapshot>() { snapshot2, snapshot1 };
            _mock.Setup(repo => repo.FindWithAllSnapshots(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.GetAllSnapshots(userId: 2, noteId: 1);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
        }
    }
}

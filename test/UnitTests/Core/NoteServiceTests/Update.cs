using Moq;
using Noteapp.Core.Dtos;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Update
    {
        private readonly Mock<INoteRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _created = new DateTime(2021, 1, 1);
        private readonly DateTime _updated = new DateTime(2021, 2, 2);

        public Update()
        {
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == _updated);
        }

        [Fact]
        public async Task UpdatesNoteText()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "original text",
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Text = "updated text"
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Update(userId: 1, noteId: 1, noteRequest);

            // Assert
            _mock.Verify(repo => repo.Update(note), Times.Once);
            Assert.Equal("updated text", note.Text);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
            Assert.Equal(_updated, note.Updated, TimeSpan.Zero);
        }

        [Fact]
        public async Task UpdatesNoteProperties()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = true,
                Locked = true,
                Archived = false
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Pinned = false,
                Locked = true,
                Archived = true
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Update(userId: 1, noteId: 1, noteRequest);

            // Assert
            Assert.False(note.Pinned);
            Assert.True(note.Locked);
            Assert.True(note.Archived);
            _mock.Verify(repo => repo.Update(note), Times.Once);
        }

        [Fact]
        public async Task SetsPublicUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = null
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Published = true
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Update(userId: 1, noteId: 1, noteRequest);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(note.PublicUrl));
            _mock.Verify(repo => repo.Update(note), Times.Once);
        }

        [Fact]
        public async Task UnsetsPublicUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Published = false
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            await noteService.Update(userId: 1, noteId: 1, noteRequest);

            // Assert
            Assert.Null(note.PublicUrl);
            _mock.Verify(repo => repo.Update(note), Times.Once);
        }

        [Fact]
        public async Task ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "original text",
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Text = "updated text"
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Update(userId: 1, noteId: 2, noteRequest);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
            _mock.Verify(repo => repo.Update(note), Times.Never);
        }

        [Fact]
        public async Task ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "original text",
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Text = "updated text"
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Update(userId: 2, noteId: 1, noteRequest);

            // Assert
            await Assert.ThrowsAsync<NoteNotFoundException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
            _mock.Verify(repo => repo.Update(note), Times.Never);
        }

        [Fact]
        public async Task ThrowsGivenNoteIsLocked()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Locked = true
            };
            var noteSnapshot = new NoteSnapshot()
            {
                NoteId = 1,
                Text = "original text",
                Created = _created
            };
            note.CurrentSnapshot = noteSnapshot;

            var noteRequest = new NoteRequest()
            {
                Text = "updated text"
            };

            _mock.Setup(repo => repo.FindWithCurrentSnapshot(1)).ReturnsAsync(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Func<Task> act = async () => await noteService.Update(userId: 1, noteId: 1, noteRequest);

            // Assert
            await Assert.ThrowsAsync<NoteLockedException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
            _mock.Verify(repo => repo.Update(note), Times.Never);
        }
    }
}

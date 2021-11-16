using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class Update
    {
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void UpdatesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var created = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = created,
                Updated = created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var updated = new DateTime(2021, 2, 2);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider(updated));

            // Act
            noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Equal("updated text", _noteRepository.Notes.Single().Text);
            Assert.Equal(updated, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var created = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = created,
                Updated = created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var updated = new DateTime(2021, 2, 2);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider(updated));

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 2, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var created = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = created,
                Updated = created,
                Text = "original text"
            };
            _noteRepository.Notes.Add(note);

            var updated = new DateTime(2021, 2, 2);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider(updated));

            // Act
            Action act = () => noteService.Update(userId: 2, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNoteIsLocked()
        {
            // Arrange
            var created = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = created,
                Updated = created,
                Text = "original text",
                Locked = true
            };
            _noteRepository.Notes.Add(note);

            var updated = new DateTime(2021, 2, 2);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider(updated));

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteLockedException>(act);
            Assert.Equal("original text", _noteRepository.Notes.Single().Text);
            Assert.Equal(created, _noteRepository.Notes.Single().Updated, TimeSpan.Zero);
            Assert.Equal(created, _noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }
    }
}

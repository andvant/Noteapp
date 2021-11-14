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
        [Fact]
        public void UpdatesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var createdDateTime = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = createdDateTime,
                LastModified = createdDateTime,
                Text = "original text"
            };
            noteRepository.Notes.Add(note);

            var updatedDateTime = new DateTime(2021, 2, 2);
            var noteService = new NoteService(noteRepository, new DateTimeProvider(updatedDateTime));

            // Act
            noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Equal("updated text", noteRepository.Notes.Single().Text);
            Assert.Equal(updatedDateTime, noteRepository.Notes.Single().LastModified, TimeSpan.Zero);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void DoesNotUpdateGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var createdDateTime = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = createdDateTime,
                LastModified = createdDateTime,
                Text = "original text"
            };
            noteRepository.Notes.Add(note);

            var updatedDateTime = new DateTime(2021, 2, 2);
            var noteService = new NoteService(noteRepository, new DateTimeProvider(updatedDateTime));

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 2, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", noteRepository.Notes.Single().Text);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().LastModified, TimeSpan.Zero);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void DoesNotUpdateGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var createdDateTime = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = createdDateTime,
                LastModified = createdDateTime,
                Text = "original text"
            };
            noteRepository.Notes.Add(note);

            var updatedDateTime = new DateTime(2021, 2, 2);
            var noteService = new NoteService(noteRepository, new DateTimeProvider(updatedDateTime));

            // Act
            Action act = () => noteService.Update(userId: 2, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", noteRepository.Notes.Single().Text);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().LastModified, TimeSpan.Zero);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }

        [Fact]
        public void DoesNotUpdateGivenNoteIsLocked()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var createdDateTime = new DateTime(2021, 1, 1);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = createdDateTime,
                LastModified = createdDateTime,
                Text = "original text",
                Locked = true
            };
            noteRepository.Notes.Add(note);

            var updatedDateTime = new DateTime(2021, 2, 2);
            var noteService = new NoteService(noteRepository, new DateTimeProvider(updatedDateTime));

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteLockedException>(act);
            Assert.Equal("original text", noteRepository.Notes.Single().Text);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().LastModified, TimeSpan.Zero);
            Assert.Equal(createdDateTime, noteRepository.Notes.Single().Created, TimeSpan.Zero);
        }
    }
}

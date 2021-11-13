using Noteapp.Api.Data;
using Noteapp.Api.Entities;
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
    public class Unlock
    {
        [Fact]
        public void UnlocksNote()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.Unlock(userId: 1, noteId: 1);

            // Assert
            Assert.True(success);
            Assert.False(note.Locked);
        }
    }
}
